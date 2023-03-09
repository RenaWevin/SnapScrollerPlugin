# SnapScrollerPlugin

一個Unity的自動定位卷軸插件<br>
A snap type scroller plugin for unity.

**這個專案尚未完成。<br>This project is still incomplete.**

## 目前已知的BUG與問題
* 在拉到最後一項之後又繼續往外拉ScrollPosition時(>1)，有可能會導致Cell被回收而使Content大小不合
  * 稍微拖曳ScrollRect後就會恢復正常
* 在透過Manager變更Data資料量後，呼叫JumpToTop似乎有可能會造成Cell顯示錯誤
  * 稍微拖曳ScrollRect後就會恢復正常