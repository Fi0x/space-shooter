using System;

namespace Helper
{
    public static class SceneManagerUtils
    {
        public enum SceneId
        {
            Startup = 0,
            StandardLevel = 1,
            BossLevel1 = 2
        }

        public static int AsInt(this SceneId sceneId)
        {
            return (int) sceneId;
        }

        public static SceneId AsSceneId(this int sceneId)
        {
            return sceneId switch
            {
                (int)SceneId.Startup => SceneId.Startup,
                (int)SceneId.StandardLevel => SceneId.StandardLevel,
                (int)SceneId.BossLevel1 => SceneId.BossLevel1,
                _ => throw new NotImplementedException()
            };
        }
    }
}