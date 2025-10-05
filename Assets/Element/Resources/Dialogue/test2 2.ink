VAR playerName = "Alice"
#Tachie1_Character:Kuma #speaker:Kuma #bgm:RickRoll #background:TestTrain
#Tachie2_Move:((-450,0),0.01)
臣亮言：先帝創業未半，而中道崩殂。今天下三分，益州疲弊，此誠危急存亡之秋也。然侍衛之臣，不懈於內；
#speaker:Kuma 
#Tachie1_Character:Kuma #Tachie1_Emotion:Laugh #Tachie1_Move:((500,0),0.5)
#Tachie2_Character:Sirius #Tachie2_Emotion:Angry  #Tachie2_Behavior:Shake
忠志之士，忘身於外者，蓋追先帝之殊遇，欲報之於陛下也。誠宜開張聖聽，以光先帝遺德，恢宏志士之氣；不宜妄自菲薄，引喻失義，以塞忠諫之路也。
#speaker:Kuma #Tachie1_Behavior:Jump
又是新的一天{playerName}
#speaker:Kuma #emotion:Laugh #Tachie1_Behavior:Flip
做人就是要好好工作，所以趕快去工作吧!!
#speaker:Kuma #emotion:Laugh #bgm:StarlightParade #background:NormalStage
今天要做些甚麼?
 * [來訓練吧!]
 ->train
 * [來種田吧!]
 ->farm
===train===
#speaker:Karo #emotion:Angry
{(train_vo||train_da||train_vi):還要訓練甚麼嗎|"今天要訓練哪項能力呢?"}
+[歌唱]
->train_vo
+[舞蹈]
->train_da
+[表現力]
->train_vi
+{TURNS_SINCE(->train)}[沒了]
->ending
=train_vo
    做了發聲練習
->train  
=train_da
    做了基礎舞步
->train
=train_vi
    訓練了氣場
->train
===farm===
- They lived happily ever after.
-> ending
===ending===
-> END