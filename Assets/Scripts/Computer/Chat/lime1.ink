你好 #speaker: Friend
初次見面 #speaker: Friend
你好啊 #speaker: Player
很高興認識你 #speaker: Player
我們要不要找時間一起吃個飯？ #speaker: Friend
沒問題，你想吃什麼？ #speaker: Player
這個嘛……交給你選吧，你喜歡吃什麼？ #speaker: Friend
* 台式小吃 #speaker: Player
  -> Taiwanese_Food
* 日式料理 #speaker: Player
  -> Japanese_Food
* 美式餐點 #speaker: Player
  -> American_Food

=== Taiwanese_Food ===
你想吃哪種台式小吃？ #speaker: Friend
* 滷肉飯 #speaker: Player
-> End_Section
* 蚵仔麵線 #speaker: Player
-> End_Section
* 雞排加珍奶 #speaker: Player
-> End_Section

=== Japanese_Food ===
你想吃哪種日式料理？ #speaker: Friend
* 拉麵 #speaker: Player
-> End_Section
* 壽司 #speaker: Player
-> End_Section
* 丼飯 #speaker: Player
-> End_Section

=== American_Food ===
你想吃哪種美式餐點？ #speaker: Friend
* 漢堡 #speaker: Player
-> End_Section
* 牛排 #speaker: Player
-> End_Section
* 早午餐 #speaker: Player
-> End_Section

=== End_Section ===
聽起來很讚，那就吃這個吧！ #speaker: Friend
-> END