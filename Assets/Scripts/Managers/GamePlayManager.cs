﻿using UnityEngine;
using Views;

namespace Managers
{
    public class GamePlayManager : MonoBehaviour
    {
        public static CollisionHandlingManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this.gameObject.GetComponent<CollisionHandlingManager>();
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(this.gameObject);
                }
            }
        }
        private void Start()
        {
            CollisionHandlingManager.Instance.TriggerEnter += CollisionHandlingManager_TriggerEnter;
            CollisionHandlingManager.Instance.TriggerExit += CollisionHandlingManager_TriggerExit;
            CollisionHandlingManager.Instance.CollisionEnter += CollisionHandlingManager_CollisionEnter;
            CollisionHandlingManager.Instance.CollisionExit += CollisionHandlingManager_CollisionExit;
        }
        private void OnDestroy()
        {
            CollisionHandlingManager.Instance.TriggerEnter -= CollisionHandlingManager_TriggerEnter;
            CollisionHandlingManager.Instance.TriggerExit -= CollisionHandlingManager_TriggerExit;
            CollisionHandlingManager.Instance.CollisionEnter -= CollisionHandlingManager_CollisionEnter;
            CollisionHandlingManager.Instance.CollisionExit -= CollisionHandlingManager_CollisionExit;
        }

        private void TryHandleBaitFishCollisionEnter(IBaseView baseView1, IBaseView baseView2)
        {
            var baitView = baseView1 as BaitView;
            var fishView = baseView2 as FishView;
            if (baitView == null ||
                fishView == null)
            {
                baitView = baseView2 as BaitView;
                fishView = baseView1 as FishView;
            }
            if (baitView == null ||
                fishView == null)
            {
                return;
            }

            FishingManager.Instance.FishOnBait = fishView;
        }
        private void TryHandleBaitFishCollisionExit(IBaseView baseView1, IBaseView baseView2)
        {
            var baitView = baseView1 as BaitView;
            var fishView = baseView2 as FishView;
            if (baitView == null ||
                fishView == null)
            {
                baitView = baseView2 as BaitView;
                fishView = baseView1 as FishView;
            }
            if (baitView == null ||
                fishView == null)
            {
                return;
            }

            FishingManager.Instance.FishOnBait = null;
        }

        private void CollisionHandlingManager_CollisionEnter(IBaseView baseView1, IBaseView baseView2)
        {
            TryHandleBaitFishCollisionEnter(baseView1, baseView2);
        }
        private void CollisionHandlingManager_CollisionExit(IBaseView baseView1, IBaseView baseView2)
        {
            TryHandleBaitFishCollisionExit(baseView1, baseView2);
        }

        private void CollisionHandlingManager_TriggerEnter(IBaseView baseView1, IBaseView baseView2)
        {
            TryHandleBaitFishCollisionEnter(baseView1, baseView2);
        }
        private void CollisionHandlingManager_TriggerExit(IBaseView baseView1, IBaseView baseView2)
        {
            TryHandleBaitFishCollisionExit(baseView1, baseView2);
        }
    }
}