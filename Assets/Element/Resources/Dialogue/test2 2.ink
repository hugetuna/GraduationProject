VAR playerName = "Alice"
又是新的一天{playerName}

 * [來訓練吧!]
 ->train
 * [來種田吧!]
 ->farm
===train===
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
    -> END
===ending===
-> END