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

        private Color primaryColor = new Color(1.0f, 1.0f, 1.0f, 1f);
        private Color regularColor = new Color(1.0f, 1.0f, 1.0f, 0.1f);
        

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

        private Vector3? GetPosition()
        {
            var response = this.Parent.GetPredictedTargetLocation();
            if (!response.HasValue)
            {
                return null;
            }

            /*if (this.IsVisible != response.Value.canHit)
            {
                this.isVisible = response.Value.canHit;
            }*/
            
            return response.Value.position;
        }

        public void NotifyAboutNewScore(float score)
        {
            
        }
    }
}
