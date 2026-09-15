using System;
using CookingSimulator.Core;
using UnityEngine;
using CookingSimulator.Scripts.Core;
using CookingSimulator.Scripts.UI;
using CookingSimulator.Scripts.UI.World;
using Unity.VisualScripting;

namespace CookingSimulator.Scripts.Gameplay
{
    public class CustomerWindow : StationBase
    {
        [SerializeField] private OrderTicketView ticket;
        [SerializeField] private FloatingScorePopup popup;

        private OrderService orders;
        private int index = -1;

        public void Bind(OrderService service, int slotIndex)
        {
            orders = service;
            index = slotIndex;

            orders.OrderSpawned += OnOrderSpawned;
            orders.OrderProgressed += OnOrderProgressed;
            orders.OrderCompleted += OnOrderCompleted;
            orders.OrderCleared += OnOrderCleared;
        }
        private void OnDestroy()
        {
            if (orders == null) return;
            orders.OrderSpawned -= OnOrderSpawned;
            orders.OrderProgressed -= OnOrderProgressed;
            orders.OrderCompleted -= OnOrderCompleted;
            orders.OrderCleared -= OnOrderCleared;
        }

        public override bool CanInteract(PlayerHands hands)
        {
            if(orders == null || !orders.HasOrder(index)) return false;
            return orders.GetOrder(index).Wants(hands.Held);
        }

        public override void Interact(PlayerHands hands)
        {
            DeliveryResult result = orders.TryDeliver(index, hands.Held);
            if (!result.Accepted) return;
            hands.Clear();
        }

        private void OnOrderSpawned(int slot)
        {
            if (slot != index) return;
            ticket.Bind(orders.GetOrder(index));
        }

        private void OnOrderProgressed(int slot)
        {
            if (slot != index) return;
            ticket.RefreshFillState();
        }

        private void OnOrderCompleted(int slot, int score)
        {
            if (slot != index) return;
            popup.Play(score);
        }
        private void OnOrderCleared(int slot)
        {
            if (slot != index) return;
            ticket.Hide();
        }

        private void LateUpdate()
        {
            if (orders == null || !orders.HasOrder(index)) return;
            ticket.RefreshTimer(orders.ElapsedFor(index), orders.ProjectedScoreFor(index));
        }
    }
}