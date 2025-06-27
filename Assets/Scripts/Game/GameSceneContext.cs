using UnityEngine;
using Zenject;

namespace Game
{
    public class GameSceneContext : MonoInstaller
    {
        [SerializeField] private GridManager gridManager;

        public override void InstallBindings()
        {
            Container.Bind<GridManager>().FromInstance(gridManager).AsSingle();
        }
    }
}