using Ruanmou.Advaned.Lottery.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ruanmou.Advaned.Lottery
{
    /// <summary>
    /// 多线程双色球项目:
    /// 双色球：投注号码由6个红色球号码和1个蓝色球号码组成。
    /// 红色球号码从01--33中选择（不重复）
    /// 蓝色球号码从01--16中选择（可以跟红球重复）
    /// 7个球杂乱无章的变化：球的号码来自于复杂数据计算，
    ///                      可能需要纳斯达克股票指数+全球气象指数+。。。（费时间）
    /// 
    /// 随机的规则是远程获取一个数据的，这个会有较长的时间损耗
    /// 多线程
    /// </summary>
    public partial class frmSSQ : Form
    {
        public frmSSQ()
        {
            //CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.btnStart.Enabled = true;
            this.btnStop.Enabled = false;
        }

        #region Data 
        /// <summary>
        /// 红球集合
        /// </summary>
        private string[] RedNums =
        {
            "01","02","03","04","05","06","07","08","09","10",
            "11","12","13","14","15","16","17","18","19","20",
            "21","22","23","24","25","26","27","28","29","30",
            "31","32","33"
        };

        /// <summary>
        /// 蓝球集合
        /// </summary>
        private string[] BlueNums =
        {
            "01","02","03","04","05","06","07","08","09","10",
            "11","12","13","14","15","16"
        };

        private static readonly object frmSSQ_Lock = new object();
        private bool IsGoOn = true;

        private List<Task> taskList = new List<Task>();
        #endregion

        #region UI
        /// <summary>
        /// 点击开始：
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnStart.Text = "运行ing";
                this.btnStart.Enabled = false;
                this.IsGoOn = true;
                this.taskList = new List<Task>();
                this.lblBlue.Text = "00";
                this.lblRed1.Text = "00";
                this.lblRed2.Text = "00";
                this.lblRed3.Text = "00";
                this.lblRed4.Text = "00";
                this.lblRed5.Text = "00";
                this.lblRed6.Text = "00";

                //Console.WriteLine(new Random().Next(0, 15));//当前的毫秒作为种子
                //Console.WriteLine(new Random().Next(0, 15));//同一时间的随机不靠谱
                //Console.WriteLine(new Random().Next(0, 15));
                //Console.WriteLine(new Random().Next(0, 15));

                Thread.Sleep(1000);
                TaskFactory taskFactory = new TaskFactory();
                foreach (var control in this.gboSSQ.Controls)
                {
                    if (control is Label)
                    {
                        Label lbl = (Label)control;
                        if (lbl.Name.Contains("Blue"))
                        {
                            taskList.Add(taskFactory.StartNew(() =>
                           {
                               while (this.IsGoOn)
                               {
                                   int indexNum = new RandomHelper().GetRandomNumberLong(0, BlueNums.Length);// new Random().Next(0, 15);
                                   string sNumber = this.BlueNums[indexNum];
                                   this.UpdateLbl(lbl, sNumber);
                               }
                           }));
                        }
                        else//红球
                        {
                            taskList.Add(taskFactory.StartNew(() =>
                            {
                                while (this.IsGoOn)
                                {
                                    int indexNum = new RandomHelper().GetRandomNumberLong(0, this.RedNums.Length);// new Random().Next(0, 15);
                                    string sNumber = this.RedNums[indexNum];
                                    lock (frmSSQ_Lock)
                                    {
                                        if (this.IsExsitRed(sNumber))
                                        {
                                            continue;//重复了就放弃更新，重新获取
                                        }
                                        this.UpdateLbl(lbl, sNumber);
                                        //任何小概率的坏事儿，在长期运行的时候，总会发生的
                                    }
                                }
                            }));
                            //一个一个出，重复就再谣
                            //写个静态的集合来 存一下：
                            //锁
                            //只在最后结束的时候去找出可能存在的重复项 然后去再找一个新的   博彩的不能接受

                            //一次产生6个--套路可以，也不好 
                            //先取6个不同的放到集合中，然后再用这6个来做线程 ---从数据上隔离，性能最高，但是有黑幕
                            //每次赋值前再次遍历控件取值啊，如果有这个值就重新取值
                        }
                    }
                }

                //Thread.Sleep(3000);//直接等  不靠谱
                //this.btnStop.Enabled = true;//正确的时机打开
                //while (true)
                //{
                //    Thread.Sleep(1000);//死锁：主线程等着子线程更新数据；子线程又等着主线程来更新
                //    if (!this.IsExsitRed("00") && !this.lblBlue.Text.Equals("00"))
                //    {
                //        this.btnStop.Enabled = true;
                //        break;
                //    }
                //}
                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        if (!this.IsExsitRed("00") && !this.lblBlue.Text.Equals("00"))
                        {
                            this.Invoke(new Action(() =>
                            {
                                this.btnStop.Enabled = true;
                            }));
                            break;
                        }
                    }
                });

                taskFactory.ContinueWhenAll(this.taskList.ToArray(), tArray => this.ShowResult());
            }
            catch (Exception ex)
            {
                Console.WriteLine("双色球启动出现异常：{0}", ex.Message);
            }
        }

        private bool IsExsitRed(string sNumber)
        {
            foreach (var control in this.gboSSQ.Controls)
            {
                if (control is Label)
                {
                    Label lbl = (Label)control;
                    if (lbl.Name.Contains("Red") && lbl.Text.Equals(sNumber))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 点击结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            this.btnStart.Enabled = true;
            this.btnStop.Enabled = false;
            this.IsGoOn = false;
            //Task.WaitAll(this.taskList.ToArray());//主线等着全部任务完成；任务还在找主线程干活；
            //可以包一层

            //this.ShowResult();//错误结果
        }

        /// <summary>
        /// 弹框提示数据
        /// </summary>
        private void ShowResult()
        {
            MessageBox.Show(string.Format("本期双色球结果为：{0} {1} {2} {3} {4} {5}  蓝球{6}"
                , this.lblRed1.Text
                , this.lblRed2.Text
                , this.lblRed3.Text
                , this.lblRed4.Text
                , this.lblRed5.Text
                , this.lblRed6.Text
                , this.lblBlue.Text));
        }
        #endregion


        #region PrivateMethod
        /// <summary>
        /// 更新界面
        /// </summary>
        /// <param name="lbl"></param>
        /// <param name="text"></param>
        private void UpdateLbl(Label lbl, string text)
        {
            if (lbl.InvokeRequired)//启动form时候 去设置一个非线程安全的属性
            {
                this.Invoke(new Action(() =>
                {
                    //if (this.IsGoOn)//不延迟，点了停止，就不会再更新,放弃了最后那次更新
                    {
                        lbl.Text = text;
                        //Thread.Sleep(2000);
                        Console.WriteLine($"当前UpdateLbl线程id{Thread.CurrentThread.ManagedThreadId}");
                    }
                }));//交给UI线程去更新
            }
            else
            {
                lbl.Text = text;
            }
        }
        #endregion
    }

}
