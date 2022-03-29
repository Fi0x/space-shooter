#nullable enable
using System.Runtime.CompilerServices;
using Manager;
using UI.Ui3D;
using UnityEngine;

namespace Targeting
{
    public class TargetableUIObject : Ui3DElement
    {
        public Targetable Parent { get; private set; } = null!;
        
        private readonly Color primaryColor = new Color(1.0f, 1.0f, 1.0f, 1f);
        private readonly Color regularColor = new Color(1.0f, 1.0f, 1.0f, 0.1f);
        private readonly Color primaryColorTooFarAway = new Color(0.6f, 0.6f, 0.0f, 1.0f);
        private readonly Color regularColorTooFarAway = new Color(0.5372f, 0.4313f, 0.0f, 0.1f);
        

        private SpriteRenderer spriteRenderer = null!;
        private bool isVisible;

        public void Init(Targetable parent)
        {
            this.Parent = parent;
            this.Parent.PrimaryTargetStateChangeEvent += this.HandleParentPrimaryStateChangedEvent;
            this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            this.spriteRenderer.color = regularColor;
            this.HandleParentPrimaryStateChangedEvent(this.Parent.IsPrimaryTarget);
            
            GameManager.Instance.Player.GetComponent<Ui3DManager>().NotifyAboutNewElement(this);
        }

        public void NotifyAboutParentBeingDestroyed()
        {
            if (this.Parent)
            {
                this.Parent.PrimaryTargetStateChangeEvent -= this.HandleParentPrimaryStateChangedEvent;
            }
            GameManager.Instance.Player.GetComponent<Ui3DManager>().RemoveElement(this);
            Destroy(this.gameObject);
        }

        private void HandleParentPrimaryStateChangedEvent(bool isParentPrimaryTarget)
        {
            this.canHitState = null; // as a result, the correct color will be applied the next tick
            var manager = GameManager.Instance.TargetableManager;
            var spriteToUse = isParentPrimaryTarget ? manager.TargetableActiveSprite : manager.TargetableInactiveSprite;

            this.spriteRenderer.sprite = spriteToUse;
            this.spriteRenderer.color = isParentPrimaryTarget ? this.primaryColor : this.regularColor;

        }

        protected override Vector3? DesiredPosition => this.GetPosition();

        protected bool IsVisible
        {
            get => isVisible;
            set
            {
                this.spriteRenderer.enabled = value;
                isVisible = value;
            }
        }

        private bool? canHitState = null;

        private Vector3? GetPosition()
        {
            var response = this.Parent.GetPredictedTargetLocation();

            if (this.IsVisible != response.HasValue)
            {
                this.IsVisible = response.HasValue;
            }
            
            if(response.HasValue)
            {
                if (!canHitState.HasValue || canHitState != response.Value.canHit)
                {
                    canHitState = response.Value.canHit;
                    if (canHitState.Value)
                    {
                        this.spriteRenderer.color = this.Parent.IsPrimaryTarget ? this.primaryColor : this.regularColor;
                    }
                    else
                    {
                        this.spriteRenderer.color = this.Parent.IsPrimaryTarget
                            ? this.primaryColorTooFarAway
                            : this.regularColorTooFarAway;
                    }
                }
            }

            return response?.position;
        }
    }
}
