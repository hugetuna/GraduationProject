VAR playerName = "Alice"
是人類用來記錄和傳播語言的書寫符號體系，可單獨或經組合表達某種或某些語意信息。書寫體(writing system)
又是新的一天{playerName}
做人就是要好好工作，所以趕快去工作吧!!
今天要做些甚麼?
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
-> ending
===ending===
-> END