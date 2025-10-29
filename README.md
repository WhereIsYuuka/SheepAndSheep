# SheepAndSheep（Unity 三消/叠层消除）

一款基于 Unity 的“叠层三消”玩法原型：中心棋盘按层级递减塔式生成，四周可选生成条状的 Extra 区；只能点击未被上一层覆盖的棋子；将同值的 3 个棋子合并消除，全部消完即胜利，收纳栏达到上限判负。

> 引擎：Unity 2021.3.45f1c1（URP 12）



## 游戏玩法

- 点击未覆盖的棋子，将其放入收纳栏
- 收纳栏：同值 3 个自动消除
- 失败：收纳栏满（默认 7）
- 胜利：全部清空

[<img src="https://s21.ax1x.com/2025/10/30/pVxq69U.png" alt="pVxq69U.png" style="zoom: 40%;" />](https://imgchr.com/i/pVxq69U)[<img src="https://s21.ax1x.com/2025/10/30/pVxqDA0.jpg" alt="pVxqDA0.jpg" style="zoom: 40%;" />](https://imgchr.com/i/pVxqDA0)



## 关键实现

### 覆盖判定
上层为 `l + 1` ，行列为 `row - upLayer / col - upLayer`

检查上层 5 个相对位置是否存在激活 Cell，存在即视为被覆盖

### 生成
中心区：按层数生成递减矩阵

Extra：按配置方向/数量/间距，从起点依次生成

生成完毕后统一刷新可交互状态（避免“先生成下层时上层未生成”的误判）

### 三消逻辑
放入栏位时，若当前位置前后已有连续两个同值，则触发合并动画并回收三者；否则按规则插入栏位并排列动画

### 数值分配（必为 3 的倍数）
先按组等分，再逐组补齐，保证 `values.Count == allCells.Count` 且“每种值数量 % 3 == 0”，最后洗牌分配

### 对象池
Pool<Cell>统一负责 `GetObject/ReturnObject`，避免频繁 Instantiate/Destroy

### 自定义 Inspector

统计主区 Cell 数

自动计算推荐 Extra 数量（补齐到 3 的倍数）
