
#speaker:Manager #background:Street
......
#speaker:Manager 
今天要去新的公司...
#speaker:Manager 
有點緊張呢，已經三年沒正式工作了。
#speaker:Manager 
上一份工作...
#speaker:Manager #background:NormalStage
...
#speaker:Manager 
（——自從那次被炒之後）
#speaker:Manager 
（不只遞出的履歷全都沒有消息，身邊還出現了奇怪的東西）
#speaker:Manager #background:Street
#font:Shake
（別想了!）
#speaker:Manager
這三年都靠著兼職艱難地撐過，好不容易有公司願意雇用我，一定要好好努力！
#speaker:Manager
雖然環境好像不怎麼好就是了...
#speaker:Empty #background:Office
#sfx:knock
（叩叩）
#speaker:Boss #background:Office #font:Big
請進！！！！！！！！！
#speaker:Boss #background:Office #font:Normal
你好呀新來的！正式向你自我介紹，我是這間小小事務所「Star Sprout」的老闆，請多多指教！
#speaker:Manager
(這個老闆...好不靠譜的感覺...)
#speaker:Boss 
那麼馬上進入正題，你還記得我們之前談好的工作內容吧，知道那個節目的名稱嗎？
#speaker:Manager 
* [...NEO Polaris?]
->NEOPOLARIS
* [...IDOL PACT?]
->IDOLPACT
* [...FIRERA?]
->FIRERA
===NEOPOLARIS===
#speaker:Boss
沒錯，NEO Polaris，那個世界級的偶像徵選企劃。
->Main

===IDOLPACT===
#speaker:Boss
嗯......我是這麼說的嗎？哈哈哈，可能是我記錯了。但IDOL PACT這名字聽起來就不會紅
#speaker:Boss
咳咳，我們回到正題
->Main

===FIRERA===
#speaker:Boss
嗯......我是這麼說的嗎？哈哈哈，可能是我記錯了。但FIRERA這名字聽起來就不會紅
#speaker:Boss
咳咳，我們回到正題
->Main
===Main===
#speaker:Boss
身為經紀人，你需要培養我們事務所的偶像團體參加NEO Polaris這個選秀節目並成功出道！
#speaker:Manager 
（「NEO Polaris」，現正火熱的世界級偶像徵選企劃）
#speaker:Manager
（由知名娛樂公司與電視台聯手打造的大型舞台，吸引了許多懷抱明星夢的少女們，一同爭奪出道的機會）
#speaker:Boss
在前幾期的節目當中，有些一開始毫不起眼的團體，卻一步步累積實力與人氣，最後成為成功出道的黑馬。
#speaker:Boss
而且還是少數開放所有公司與個人練習生公平競爭的選秀舞台！
#speaker:Boss
這對我們這種小公司來說可是一根救命稻草！
#speaker:Boss
我家事務所的孩子們，說不定也能變得像那些女孩一樣。
#speaker:Boss
所以...雖然這裡的財務狀況相當吃緊，面臨的競爭也更加激烈，但我們已經決定孤注一擲，把整個公司都押在這次選秀上了！
#speaker:Manager
#font:Shake
（欸！？等一下，這個情況是不是不太妙？？？）
#speaker:Boss
這就是我們聘請你的原因，我們需要你帶領公司的3位練習生組成一個團隊參賽獲得冠軍，要是這團偶像再沒辦法出道的話，我們公司就要倒閉了喔！
#speaker:Boss
這些就是你可以選擇的練習生們的資料，放心吧，我家的孩子都很努力的。
#speaker:Boss
希望你能讓她們成為獨當一面的偶像，拜託了！！！
->ending
===hello===
#speaker:Kuma 
#Tachie1_Character:Kuma #Tachie1_Emotion:Laugh #Tachie1_Move:((500,0),0.5)
#Tachie2_Character:Sirius #Tachie2_Emotion:Angry  #Tachie2_Behavior:Shake
忠志之士，忘身於外者，蓋追先帝之殊遇，欲報之於陛下也。誠宜開張聖聽，以光先帝遺德，恢宏志士之氣；不宜妄自菲薄，引喻失義，以塞忠諫之路也。
#speaker:Kuma #Tachie1_Behavior:Jump
又是新的一天
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