﻿HIT教务处小助手
======================

起因&经过&结果：
----------
表示鄙视教务处网站，加上前期有些日子闲的慌，于是有了这个，最近简单整理了下，稍改进了下，放到这里，分享给大家

主要功能：
----------
 - 1.登录教务处网站，计算成绩（包括学分绩以及GPA，当然，你必须在校内网）
	 - 1.1.图表展示各学期分数情况
	 - 1.2.导入导出当前成绩数据，并提供加密
	 - 1.3.登录时可选保存密码，提供简单加密
 - 2.待开发。。。

注意:
---------
 - 一般情况下，点击程序是可以直接运行的，如果有问题的话，装个.NET Framework 2.0即可
 - 软件纯C#打造，比较粗糙，见谅

软件中的GPA算法：
----------------
（分级算法，即将每门课的分数按照下面的算法换算为相应等级，然后和学分加权平均）

北大：
-----------
	90-100  4.0   
	85-89   3.7   
	82-84   3.3   
	78-81   3.0   
	75-77   2.7   
	72-74   2.3   
	68-71   2.0   
	64-67   1.5   
	60-63   1.0   
	<60     0

标准：
-----------
	90-100    4.0  
	80-89     3.0   
	70-79     2.0  
	60-69     1.0

改进标准：
-----------
	85-100     4.0  
	70-85      3.0  
	60-70      2.0  
	0-60       0

4.3标准：
-----------
	90-100  4.3  
	85-90   4.0  
	80-85   3.7  
	75-80   3.3  
	70-75   3  
	65-70   2.7  
	60-65   2.3

浙大(比较凶残)：
-----------
	score > 85	4;
	else	4 - (85 - s) / 10;

Licence
----------
	MIT

Questions? || Bugs?
-----------
 - <https://github.com/xizhibei/HIT_JWC_Helper/issues>
 - Xu Zhipei <xuzhipei@gmail.com>
 
 Update:
----------
 - 2012/9/15
	- 更新主界面与登录界面背景。。。请轻砸==！。。。
	- 修复成绩显示错误
	- 重构MainForm，将成绩计算等功能移动至ScoreManager统一管理
	- 导入导出格式略有变化，不兼容之前的了。。。（好吧，我有空改成xml格式的）