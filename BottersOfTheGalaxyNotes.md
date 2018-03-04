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
* Day 2 **Beat Wood League 2 (Not Achieved Yet)**
  * Types
    * Move now has PickHero type
    * Move now has Buy type
    * Move now has Sell type
  * Helper Functions
    * buyItem
    * sellItem
    * buyPotion
  * Strategy
    * Same as above, in addition:
    * If health < 200, buy potion (may need to sell off slot first).
    * If > attack range to enemy hero buy an item if enough gold and open slot.
    * Items focused on health.
