Botters of the Galaxy
=====================

Notes about Botters of the Galaxy (not adding code to the repository until after the competition).

* Day 1 **Beat Wood League 3**
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
