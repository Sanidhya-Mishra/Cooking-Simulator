# Cooking Simulator

A top-down kitchen game made in Unity. You run a small kitchen for a 3-minute shift: grab ingredients, chop and cook them into dishes, and serve customers at four order windows before they get tired of waiting. The faster you serve an order, the more points it's worth. Beat your high score.

## Play the game

The game is a ready-to-play Windows build in the [`Build`](Build) folder of this repository.

1. On this page, click the green **Code** button, then **Download ZIP**.
2. Extract the ZIP anywhere on your PC.
3. Open the extracted `Build` folder and run **`CookingSimulator.exe`**.

> Keep all files inside `Build` together. The `.exe` won't start without the `CookingSimulator_Data` folder and DLLs next to it.
>
> If Windows shows "Windows protected your PC", click **More info** then **Run anyway**. This appears for any game that isn't signed by a publisher.

**Requirements:** Windows 10 or 11 (64-bit), a keyboard or a gamepad.

## How to play

Customers show up at the four order windows. Each order asks for 2 or 3 dishes. Make them and hand them over at the right window.

1. **Pick up** an ingredient: meat, vegetable or cheese.
2. **Prepare** it at a station: chop at the **Chopping Table**, cook on the **Stove**.
3. **Deliver** the finished dish to a window whose order needs it.
4. Picked up the wrong thing? Throw it in the **Trash**, or press **Drop**.

When every dish in an order is delivered, you score the order's points **minus 1 point for every second the customer waited**. A new customer arrives at that window a few seconds later. The round ends after 3 minutes, and your best score is saved.

### Dishes

| Dish              | How to make it                                 | Points |
|-------------------|------------------------------------------------|:------:|
| Cheese Slice      | Chop cheese                                    | 10     |
| Chopped Vegetable | Chop a vegetable                               | 20     |
| Cooked Meat       | Cook meat on the stove                         | 30     |
| Cheesy Vegetable  | Chop a vegetable, then add cheese on the stove | 40     |
| Cheesy Meat       | Cook meat, then add cheese on the stove        | 50     |

Press **Tab** in game to open the recipe book.

### Controls

| Action              | Keyboard | Gamepad    |
|---------------------|----------|------------|
| Move                | W A S D  | Left stick |
| Interact / pick up  | Space    | A (South)  |
| Drop held item      | Q        | X (West)   |
| Recipe book         | Tab      | Select     |
| Pause               | Esc      | Start      |

## Open the project in Unity

- **Unity version:** 6000.4.12f1 (Unity 6)
- **Render pipeline:** Universal Render Pipeline (URP)
- **Input:** Unity Input System


Round length, number of windows, player speed and other settings are in `Assets/CookingSimulator/Data/GameConfig.asset`. You can add new dishes by creating Dish and Recipe assets and adding them to the recipe book (`RB_AllRecipes`), without changing any code.
