Setup for the demo

1. Create a new layer called "CombatText"
2. Open the BattleText/Demo/Scenes/Demo.unity scene
3. Set the following object to be in the CombatText layer

   - Text_CombatLeftSource
   - Text_CombatRightSource
   - Text_DamageSource
   - Text_NoticeSource

4. Set the culling mask on Camera component on the Camera_BattleText object
to only render the CombatText layer

5. Set the culling mask on the Camera component on the Camera_Main object
to render everything except the CombatText layer