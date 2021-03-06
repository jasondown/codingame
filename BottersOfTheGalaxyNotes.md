Botters of the Galaxy
=====================

Notes about Botters of the Galaxy (not adding code to the repository until after the competition).

* Day 1 **Goal: Beat Wood League 3 (Achieved)**
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
* Day 2 **Goal: Beat Wood League 2 (Achieved)**
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
* Day 2 con't **Bonus Goal: Beat Wood League 1 (Achieved)**
  * Strategy
    * Added ability to handle multiple heroes.
* Day 3 **Goal: Move inside top 250 (Currently mid 300s)**
  * Types
    * Added new SkillType for each available skill (3 per hero)
    * Updated Move type to allow for skills.
  * Strategy
    * Testing out Deadpool + Ironman combo
    * Added strategy for each of their skills so far.
* Day 4 **Goal: Move inside top 50 of Bronze**
  * Silver League was opened up today. I've moved to ~135 in Bronze League after many people moved to Silver.
  * Yesterday's strategy exploration lead to dropping from ~330 to ~500 overall. Ugh.
  * Started provoking closest Groot to enemy heroes via Ironman fireball (that are further away from my heroes). Extra damage for me.
  * Moved to 60th in Bronze and ~460 overall. 
  * Currently 2nd in F# overall.
* Day 5 **Goal: Move into Silver before Gold is unlocked (Achieved)**
  * Moved into top 5 bronze for a while late last night (after my log) and made a tweak before going to bed.
  * Checked today and the tweaked moved me into Silver (could've been from the patch that was released with a couple buffs, nerfs and bug fixes).
  * Changed low health strategy from hiding by the tower to hiding in the bushes near the tower.
  * Fixed a bug in the Groot provoking logic and refactored it.
  * Added MoveAttack logic.
  * Tweaked some of the skill logic.
  * Moved as high as ~150 overall, but currently sitting ~220 overall.
  * Moved back into 1st overall for F#.
* Day 6 **Goal: Beat Silver League**
  * Fix attack distances, taking hero movement speed into consideration.
  * Run away from Groot if being attacked (maybe attack him?)
* Day 7 **Goal: Beat Silver League**
  * More attack distance fixes.
  * Attack Groot if he is in attack range.
  * Getting random time outs right now that loses me matches that I win if I replay them sometimes.
* Day 8 **Goal: Beat Silver League**
  * Moved into Gold after fixing time outs (removing debug messages) about an hour before Legend is unlocked.
  * Sitting around 235 in Gold (~255 overall)
  * Didn't really have time to code much this day.
* Day 9 **Goal: top 25 in Gold**
  * There is a Hulk bug (two hulks!).
  * Trying out the dual Hulk strategy that vaulted many people.
* Day 10 **Goal: Top 25 in Gold**
  * Hulk strategy didn't work so well. 
  * Finally doing last ditch effort with Ironman and Dr. Strange.
  * ~~Sitting near the bottom of gold. Boourns.~~
  * Ironman and Dr. Strange minor tweaks moved me a little better in gold.
  * Tried Ironman + Valkyrie strategy for bush sniping + blink (if an appropriate bush is available), otherwise use Ironman and Dr. Strange in a conservative strategy (seemed to work better than aggressive).
  

## Final Result:

* 416th Overall (of 1713)
* 296th in Gold
* 2nd in F#
