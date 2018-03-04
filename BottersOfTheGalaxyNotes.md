Botters of the Galaxy
=====================

Notes about Botters of the Galaxy (not adding code to the repository until after the competition).

* Day 1 **Beat Wood League 3 (Achieved)**
   * Types
      * EntityType
      * Point
      * Entity
      * Item
      * UnitType
      * HeroType
      * Unit
      * RoundType
      * Move
   * Parsing
      * Entities
      * Items
      * Units
      * Gold
      * Team
   * Helper Functions
      * readNLines (read N lines from the console)
      * getUnitType
      * getHeroType
    * Strategy
      * Take into account starting side of board.
      * Use Ironman
      * Back off 25 behind front unit.
      * If enemy hero is in attack range, attack them.
      * Else attack nearest enemy unit.
* Day 2 **Beat Wood League 2 (Achieved)**
  * Types
    * Move now has PickHero type
    * Move now has Buy type
    * Move now has Sell type
  * Helper Functions
    * buyItem
    * sellItem
    * buyPotion
  * Strategy
    * Check if potion needed and available. Buy if necessary (may need to sell stuff first).
    * Low health strategy (either potions, hide by tower or sell stuff to buy potions).
    * Stay away from enemy tower.
    * Don't attack enemy hero unless enemy units are low in numbers (prevent aggro attack).
    * Buying items based on damage or health (decide based on hero health vs. enemy hero health).
