using Components.TreeCell;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Systems.World
{
    [UpdateAfter(typeof(WorldSystem))]
    [UpdateAfter(typeof(StepPhysicsWorld))]
    class NewTreeGrower : SystemBase
    {
        private BuildPhysicsWorld _physicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;
        private EndSimulationEntityCommandBufferSystem m_endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
            m_endSimulationEntityCommandBufferSystem =
                World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = m_endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Dependency = new CollisionEventJob
                {
                    ECB = ecb,
                    Tag = GetComponentDataFromEntity<NewSeedTag>(true),
                    TreeComponentGroup = GetComponentDataFromEntity<TreeCellComponent>(),
                    TranslationGroup = GetComponentDataFromEntity<Translation>()
                }
                .Schedule (_stepPhysicsWorld.Simulation, ref _physicsWorld.PhysicsWorld, Dependency);
            
            m_endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        [BurstCompile]
        struct CollisionEventJob : ICollisionEventsJob
        {
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public ComponentDataFromEntity<NewSeedTag> Tag;
            public ComponentDataFromEntity<TreeCellComponent> TreeComponentGroup;
            public ComponentDataFromEntity<Translation> TranslationGroup;

            public void Execute(CollisionEvent collisionEvent)
            {
                var entityA = collisionEvent.EntityA;
                var entityB = collisionEvent.EntityB;
                var entityAIsTreeCell = TreeComponentGroup.HasComponent(entityA);
                var entityBIsTreeCell = TreeComponentGroup.HasComponent(entityB);

                if (entityAIsTreeCell && entityBIsTreeCell)
                {
                    var translationA = TranslationGroup[entityA];
                    var translationB = TranslationGroup[entityB];
                    
                    ECB.DestroyEntity(0, translationA.Value.y > translationB.Value.y ? entityB : entityA);
                }
                else if (entityAIsTreeCell || entityBIsTreeCell)
                {
                    //Debug.Log("NewTree");
                }
            }
        }
    }
}

// public class NewTreeGrower : JobComponentSystem
// {
//     private BuildPhysicsWorld _physicsWorld;
//     private StepPhysicsWorld _stepPhysicsWorld;
//     private EndSimulationEntityCommandBufferSystem m_endSimulationEntityCommandBufferSystem;
//     
//     protected override void OnCreate()
//     {
//         base.OnCreate();
//
//         _physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
//         _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
//         m_endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//     }
//
//     protected override JobHandle OnUpdate(JobHandle inputDeps)
//     {
//         var ecb = m_endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
//         
//         var collisionJob = new ShopTriggerJob
//         {
//             ECB = ecb,
//             TreeComponentGroup = GetComponentDataFromEntity<TreeCellComponent>(),
//             TranslationGroup = GetComponentDataFromEntity<Translation>()
//         };
//
//         var jobShedule =
//             collisionJob.Schedule(_stepPhysicsWorld.Simulation, ref _physicsWorld.PhysicsWorld, inputDeps);
//         
//         //jobShedule.Complete();
//         
//         m_endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobShedule);
//         
//         return jobShedule;
//     }
//     
//     //[BurstCompile]
//     private struct ShopTriggerJob : ICollisionEventsJob
//     {
//         public EntityCommandBuffer.ParallelWriter ECB;
//         public ComponentDataFromEntity<TreeCellComponent> TreeComponentGroup;
//         public ComponentDataFromEntity<Translation> TranslationGroup;
//         
//         public void Execute(CollisionEvent collisionEvent)
//         {
//             var entityA = collisionEvent.EntityA;
//             var entityB = collisionEvent.EntityB;
//
//             var entityAIsTreeCell = TreeComponentGroup.HasComponent(entityA);
//             var entityBIsTreeCell = TreeComponentGroup.HasComponent(entityB);
//
//             if (entityAIsTreeCell && entityBIsTreeCell)
//             {
//                 var translationA = TranslationGroup[entityA];
//                 var translationB = TranslationGroup[entityB];
//
//                 if (translationA.Value.y > translationB.Value.y)
//                 {
//                     ECB.DestroyEntity(0, entityB);
//                 }
//                 else
//                 {
//                     ECB.DestroyEntity(0, entityA);
//                 }
//             }
//             else if (entityAIsTreeCell || entityBIsTreeCell)
//             {
//                 Debug.Log("NewTree");
//             }
//         }
//     }
// }