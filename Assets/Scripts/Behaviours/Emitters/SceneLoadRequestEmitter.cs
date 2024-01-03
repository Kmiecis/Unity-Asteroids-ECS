namespace Asteroids
{
    public class SceneLoadRequestEmitter : EntityEmitter<SceneLoadRequest>
    {
#if UNITY_EDITOR
        public UnityEditor.SceneAsset scene;

        protected override void Awake()
        {
            base.Awake();

            Emit();
        }

        protected override void OnValidate()
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(scene);
            var guid = UnityEditor.AssetDatabase.GUIDFromAssetPath(path);
            data.guid = guid;
        }
#endif
    }
}