using System;
using System.Collections.Generic;
using CookingSimulator.Scripts.Data;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace CookingSimulator.Core
{
    public readonly struct DeliveryResult
    {
        public readonly bool Accepted;
        public readonly bool CompletedOrder;
        public readonly int AwardedScore;

        public DeliveryResult(bool accepted, bool completedOrder, int awardedScore)
        {
            Accepted = accepted;
            CompletedOrder = completedOrder;
            AwardedScore = awardedScore;
        }
        public static readonly DeliveryResult Rejected = new DeliveryResult(false, false, 0);
    }

    public sealed class OrderService
    {
        private sealed class Slot
        {
            public readonly Order Order = new Order();
            public bool HasOrder;
            public float RespawnTimer;
        }
    
    private readonly GameConfig config;
    private readonly Slot[] slots;
    private readonly List<DishDefinition> menu = new List<DishDefinition>();
    private readonly List<DishDefinition> scratch = new List<DishDefinition>();
    
    private System.Random random;
    private float clock;
    private bool spawningDisabled;

    public event Action<int> OrderSpawned;
    public event Action<int> OrderProgressed;
    public event Action<int, int> OrderCompleted;
    public event Action<int> OrderCleared;
    
    public OrderService(GameConfig config)
    {
        this.config = config;
        slots = new Slot[config.WindowCount];
        for(int i = 0; i<slots.Length; i++)
            slots[i] = new Slot();
    }
    public int SlotCount => slots.Length;
    public float Clock => clock;
    
    public bool HasOrder(int index) => slots[index].HasOrder;
    public Order GetOrder(int index) => slots[index].HasOrder ? slots[index].Order : null;
    public float ElapsedFor(int index) => slots[index].HasOrder ? slots[index].Order.ElapsedAt(clock) : 0f;
    public int ProjectedScoreFor(int index) => slots[index].HasOrder? slots[index].Order.ScoreAt(clock) : 0;

    public void StartRound()
    {
        clock = 0f;
        spawningDisabled = false;
        random = new System.Random(config.OrderSeed!=0 ? config.OrderSeed: Environment.TickCount);

        menu.Clear();
        if (config.RecipeBook != null) config.RecipeBook.GetAllDishes(menu);

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].HasOrder = false;
            slots[i].RespawnTimer = 0f;
            Spawn(i);
        }
    }

    public void StopRound()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].HasOrder) continue;
            slots[i].HasOrder = false;
            slots[i].RespawnTimer = 0f;
            OrderCleared?.Invoke(i);
        }
    }

    public void Tick(float deltaTime)
    {
        clock += deltaTime;
        if(spawningDisabled) return;
        for (int i = 0; i < slots.Length; i++)
        {
            Slot slot = slots[i];
            if (slot.HasOrder)continue;
            slot.RespawnTimer -= deltaTime;
            if (slot.RespawnTimer <= 0f)
                Spawn(i);
        }
    }

    public DeliveryResult TryDeliver(int index, in KitchenItem item)
    {
        Slot slot = slots[index];
        if(!slot.HasOrder) return DeliveryResult.Rejected;
        if(!slot.Order.TryFill(item)) return DeliveryResult.Rejected;
        if (!slot.Order.IsComplete)
        {
            OrderProgressed?.Invoke(index);
            return new DeliveryResult(true, false, 0);
        }
        int score = slot.Order.ScoreAt(clock);
        slot.HasOrder = false;
        slot.RespawnTimer = config.OrderRespawnDelay;
        
        OrderCompleted?.Invoke(index, score);
        OrderCleared?.Invoke(index);
        return new DeliveryResult(true, true, score);
    }

    private void Spawn(int index)
    {
        if (menu.Count == 0)
        {
            spawningDisabled = true;
            Debug.LogError("[OrderService] GameConfig has no RecipeBook or the book has no dishes. Order spawning is disabled for this round");
            return;
        }
        int count = random.NextDouble() < config.ThreeDishChance ? 3 : 2;
        scratch.Clear();
        for(int i = 0; i<count; i++)
            scratch.Add(menu[random.Next(0, menu.Count)]);
        Slot slot = slots[index];
        slot.Order.Initialize(scratch,clock);
        slot.HasOrder = true;
        slot.RespawnTimer = 0f;
        OrderSpawned?.Invoke(index);
    }
    
    }
}