using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sevens
{
    public partial class Form1 : Form
    {

        PictureBox[] poker1 = new PictureBox[13]; //圖片陣列 //梅花
        PictureBox[] poker2 = new PictureBox[13];           //方塊
        PictureBox[] poker3 = new PictureBox[13];           //愛心
        PictureBox[] poker4 = new PictureBox[13];           //黑桃

        PictureBox[] player_poker = new PictureBox[13];  //玩家手牌的圖片陣列

        List<List<List<int>>> all_hand_card = new List<List<List<int>>>(); //手牌List [人][花色][數字]
        List<List<int>> now_card = new List<List<int>>();  //當前牌桌上的牌
        int[] all_loss = new int[4];  //紀錄所有人的蓋牌點數
        int last;  //紀錄剩下多少牌(判斷結束用)
        int round;  //輪到誰了
        int player_send_f; //玩家出牌時的花色
        int player_send_n; //玩家出牌時的點數
        int player_send_t; //玩家出牌時的Tag
        bool player_send;  //偵測玩家是否出牌

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = 850; //介面設定
            this.Height = 800;
            this.MaximumSize = new Size(850, 800);
            this.MinimumSize = new Size(850, 800);
            this.Text = "牌七(接龍)";
            label_last.Location = new Point(650, 50);
            label_round.Location = new Point(650, 110);
            label_msn.Location = new Point(650, 170);
            label_loss.Location = new Point(650, 270);
            label_ai1_loss.Location = new Point(650, 480);
            label_ai2_loss.Location = new Point(650, 510);
            label_ai3_loss.Location = new Point(650, 540);
            label_player_loss.Location = new Point(650, 570);
            label_winner.Location = new Point(650, 610);
            label_author.Location = new Point(760, 740);
            label_author.Text = "作者:張維元";
            //label1.Visible = true;
            //label2.Visible = true;


            Make_card(); 
            Wash_card();
            Give_card();

            last = 52;
            player_send_f = -1;
            player_send_n = -1;
            player_send_t = -1;
            player_send = false;

            for (int i = 0; i < 4; i++)  //找出梅花7在誰手上，他先開始
            {
                if (all_hand_card[i][0].Contains(7))
                {
                    round = i;
                    break;
                }
            }
            if (round == 3) label_round.Text = "輪到你了";
            else label_round.Text = "輪到" + (round + 1) + "號了";
            timer1.Enabled = true; //開始遊戲
        }

        public void Make_card() //生成所有的牌並放在桌上，然後隱藏
        {
            
            for (int i = 0; i < 13; i++)
            {
                poker1[i] = new PictureBox();
                poker1[i].Image = imageList1.Images[i];
                poker1[i].SizeMode = PictureBoxSizeMode.StretchImage;
                poker1[i].Width = 100;
                poker1[i].Height = 120;
                poker1[i].Location = new Point(50, 350 - 25 * i);
                poker1[i].Visible = false;
                this.Controls.AddRange(poker1);
            }
            for (int i = 0; i < 13; i++)
            {
                poker2[i] = new PictureBox();
                poker2[i].Image = imageList2.Images[i];
                poker2[i].SizeMode = PictureBoxSizeMode.StretchImage;
                poker2[i].Width = 100;
                poker2[i].Height = 120;
                poker2[i].Location = new Point(200, 350 - 25 * i);
                poker2[i].Visible = false;
                this.Controls.AddRange(poker2);
            }
            for (int i = 0; i < 13; i++)
            {
                poker3[i] = new PictureBox();
                poker3[i].Image = imageList3.Images[i];
                poker3[i].SizeMode = PictureBoxSizeMode.StretchImage;
                poker3[i].Width = 100;
                poker3[i].Height = 120;
                poker3[i].Location = new Point(350, 350 - 25 * i);
                poker3[i].Visible = false;
                this.Controls.AddRange(poker3);
            }
            for (int i = 0; i < 13; i++)
            {
                poker4[i] = new PictureBox();
                poker4[i].Image = imageList4.Images[i];
                poker4[i].SizeMode = PictureBoxSizeMode.StretchImage;
                poker4[i].Width = 100;
                poker4[i].Height = 120;
                poker4[i].Location = new Point(500, 350 - 25 * i);
                poker4[i].Visible = false;
                this.Controls.AddRange(poker4);
                
            }
        }

        private void Wash_card()  //洗牌
        {
            for (int i = 0; i < 4; i++)  //初始化手牌List
            {
                all_hand_card.Add(new List<List<int>>());
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    all_hand_card[i].Add(new List<int>());
                }
            }

            for (int i = 0; i < 4; i++)  //初始化牌桌上的牌List
            {
                now_card.Add(new List<int>());
            }

            for (int i = 0; i < 4; i++) //初始化蓋牌總數
            {
                all_loss[i] = 0;
            }

            int t1 = 0;  //記錄拿了多少牌
            int t2 = 0;
            int t3 = 0;
            int t4 = 0;
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)  //隨機分發牌
            {
                for (int j = 1; j <= 13; j++)
                {
                    
                    int t = rnd.Next(0, 4); //隨機發給四位中其中一位
                    if (t == 0)  //發給AI1
                    {
                        if (t1 < 13)
                        {
                            all_hand_card[t][i].Add(j);
                            t1++;
                        }
                        else
                        {
                            j--;
                        }
                    }
                    if (t == 1)  //發給AI2
                    {
                        if (t2 < 13)
                        {
                            all_hand_card[t][i].Add(j);
                            t2++;
                        }
                        else
                        {
                            j--;
                        }
                    }
                    if (t == 2)  //發給AI3
                    {
                        if (t3 < 13)
                        {
                            all_hand_card[t][i].Add(j);
                            t3++;
                        }
                        else
                        {
                            j--;
                        }
                    }
                    if (t == 3)  //發給玩家
                    {
                        if (t4 < 13)
                        {
                            all_hand_card[t][i].Add(j);
                            t4++;
                        }
                        else
                        {
                            j--;
                        }
                    }
                }
            }
        }

        private void Give_card()  //將玩家的牌顯示在下方
        {
            int t = 0;
            for (int i = 0; i < all_hand_card[3][0].Count; i++)
            {
                player_poker[t] = new PictureBox();
                player_poker[t].Image = imageList1.Images[all_hand_card[3][0][i]-1];
                player_poker[t].SizeMode = PictureBoxSizeMode.StretchImage;
                player_poker[t].Width = 100;
                player_poker[t].Height = 120;
                player_poker[t].Name = all_hand_card[3][0][i].ToString();
                player_poker[t].Tag = t;
                player_poker[t].Location = new Point(500 - 40 * t, 600);
                this.Controls.AddRange(player_poker);
                player_poker[t].Click += player_Click;
                player_poker[t].MouseEnter += player_MouseEnter;
                player_poker[t].MouseLeave += player_MouseLeave;
                t++;
            }
            for (int i = 0; i < all_hand_card[3][1].Count; i++)
            {
                player_poker[t] = new PictureBox();
                player_poker[t].Image = imageList2.Images[all_hand_card[3][1][i] - 1];
                player_poker[t].SizeMode = PictureBoxSizeMode.StretchImage;
                player_poker[t].Width = 100;
                player_poker[t].Height = 120;
                player_poker[t].Name = (100 + all_hand_card[3][1][i]).ToString();
                player_poker[t].Tag = t;
                player_poker[t].Location = new Point(500 - 40 * t, 600);
                this.Controls.AddRange(player_poker);
                player_poker[t].Click += player_Click;
                player_poker[t].MouseEnter += player_MouseEnter;
                player_poker[t].MouseLeave += player_MouseLeave;
                t++;
            }
            for (int i = 0; i < all_hand_card[3][2].Count; i++)
            {
                player_poker[t] = new PictureBox();
                player_poker[t].Image = imageList3.Images[all_hand_card[3][2][i] - 1];
                player_poker[t].SizeMode = PictureBoxSizeMode.StretchImage;
                player_poker[t].Width = 100;
                player_poker[t].Height = 120;
                player_poker[t].Name = (200 + all_hand_card[3][2][i]).ToString();
                player_poker[t].Tag = t;
                player_poker[t].Location = new Point(500 - 40 * t, 600);
                this.Controls.AddRange(player_poker);
                player_poker[t].Click += player_Click;
                player_poker[t].MouseEnter += player_MouseEnter;
                player_poker[t].MouseLeave += player_MouseLeave;
                t++;
            }
            for (int i = 0; i < all_hand_card[3][3].Count; i++)
            {
                player_poker[t] = new PictureBox();
                player_poker[t].Image = imageList4.Images[all_hand_card[3][3][i] - 1];
                player_poker[t].SizeMode = PictureBoxSizeMode.StretchImage;
                player_poker[t].Width = 100;
                player_poker[t].Height = 120;
                player_poker[t].Name = (300 + all_hand_card[3][3][i]).ToString();
                player_poker[t].Tag = t;
                player_poker[t].Location = new Point(500 - 40 * t, 600);
                this.Controls.AddRange(player_poker);
                player_poker[t].Click += player_Click;
                player_poker[t].MouseEnter += player_MouseEnter;
                player_poker[t].MouseLeave += player_MouseLeave;
                t++;
            }
        }

        private bool Need_loss()  //判斷玩家是否要蓋牌，需要傳回true，不需要傳回false
        {
            bool t = true;
            for (int i = 0; i < 4; i++)
            {
                if (all_hand_card[3][i].Contains(7)) t = false;
                if (now_card[i].Count != 0 && all_hand_card[3][i].Contains(now_card[i][0] - 1)) t = false;
                if (now_card[i].Count != 0 && all_hand_card[3][i].Contains(now_card[i][now_card[i].Count() - 1] + 1)) t = false;
            }
            return t;
        }

        

        private void player_Click(object sender, EventArgs e) //當玩家點擊牌時
        {
            PictureBox pic = sender as PictureBox;
            if (null == pic) return;
            int n = int.Parse(pic.Name);
            int f = n / 100;
            n = n % 100;
            //label1.Text = f.ToString();
            //label2.Text = n.ToString();
            player_send_f = f;
            player_send_n = n;
            player_send_t = (int)pic.Tag;
            player_send = true;

        }

        private void player_MouseEnter(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            int bx = pic.Location.X;
            int by = pic.Location.Y;
            pic.Location = new Point(bx, by - 10);
        }

        private void player_MouseLeave(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            int bx = pic.Location.X;
            int by = pic.Location.Y;
            pic.Location = new Point(bx, by + 10);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (last != 0)
            {
                if (round == 3) label_round.Text = "輪到你了";
                else label_round.Text = "輪到" + (round + 1) + "號了";
                label_msn.Text = "";
                label_last.Text = "剩餘牌數:" + last;
                label_loss.Text = "蓋牌總數:" + all_loss[3];

                if (Need_loss() && round == 3)
                {
                    label_msn.Text = "選擇一張蓋牌";
                }

                if (round == 0 || round == 1 || round == 2)  //AI出牌
                {
                    int flower = -1;
                    int number = -1;
                    bool can = false;
                    for (int i = 0; i < 4; i++)  //先出7
                    {
                        if (all_hand_card[round][i].Contains(7))
                        {
                            flower = i;
                            number = 7;
                            can = true;
                            break;
                        }
                    }
                    if (can == false)
                    {
                        Random rnd = new Random();
                        int t = rnd.Next(0, 4);
                        for (int i = 0; i < 4; i++)
                        {
                            if (now_card[t].Count != 0 && all_hand_card[round][t].Contains(now_card[t][0]-1)){ //先出點數小的
                                flower = t;
                                number = now_card[t][0] - 1;
                                can = true;
                                break;
                            }
                            if (now_card[t].Count != 0 && all_hand_card[round][t].Contains(now_card[t][now_card[t].Count() - 1] + 1)) //在出點數大的
                            {
                                flower = t;
                                number = now_card[t][now_card[t].Count() - 1] + 1;
                                can = true;
                                break;
                            }
                            t++;
                            if (t == 4) t = 0;
                        }
                    }
                    if (can)  //如果可以出牌
                    {
                        if (number == 7)
                        {
                            now_card[flower].Add(7);
                            all_hand_card[round][flower].Remove(7);
                            last--;
                        }
                        if (number < 7)
                        {
                            now_card[flower].Insert(0, number);
                            all_hand_card[round][flower].Remove(number);
                            last--;
                            if (flower == 0)  //為了他媽的好看一點
                            {
                                poker1[number - 1].Visible = true;
                                poker1[number - 1].BringToFront();
                            }
                            if (flower == 1)
                            {
                                poker2[number - 1].Visible = true;
                                poker2[number - 1].BringToFront();
                            }
                            if (flower == 2)
                            {
                                poker3[number - 1].Visible = true;
                                poker3[number - 1].BringToFront();
                            }
                            if (flower == 3)
                            {
                                poker4[number - 1].Visible = true;
                                poker4[number - 1].BringToFront();
                            }
                        }
                        if (number > 7)
                        {
                            now_card[flower].Add(number);
                            all_hand_card[round][flower].Remove(number);
                            last--;
                        }
                        if (flower == 0) poker1[number - 1].Visible = true;
                        if (flower == 1) poker2[number - 1].Visible = true;
                        if (flower == 2) poker3[number - 1].Visible = true;
                        if (flower == 3) poker4[number - 1].Visible = true;
                    }
                    else  //蓋牌
                    {
                        Random rnd = new Random();
                        int t = rnd.Next(0, 4);
                        for (int i = 0; i < 4; i++)
                        {
                            if(all_hand_card[round][t].Count != 0)
                            {
                                int temp = all_hand_card[round][t][0];
                                all_loss[round] += temp;
                                all_hand_card[round][t].Remove(temp);
                                label_msn.Text = (round + 1) + "號蓋牌";
                                last--;
                                break;
                            }
                            t++;
                            if (t == 4) t = 0;
                        }
                    }
                    round++;
                    if (round == 4) round = 0;
                }
                if (round == 3) //輪到玩家
                {
                    if (player_send) //玩家點擊牌
                    {
                        if (!Need_loss())
                        {
                            bool can = false;
                            if (player_send_n == 7) can = true; //當然可以出7
                            if (player_send_n < 7)
                            {
                                if (now_card[player_send_f].Count != 0 && now_card[player_send_f].Contains(player_send_n + 1))
                                {
                                    can = true;
                                }
                            }
                            if (player_send_n > 7)
                            {
                                if (now_card[player_send_f].Count != 0 && now_card[player_send_f].Contains(player_send_n - 1))
                                {
                                    can = true;
                                }
                            }
                            if (can)  //如果可以出牌
                            {
                                if (player_send_n == 7)
                                {
                                    now_card[player_send_f].Add(7);
                                    all_hand_card[3][player_send_f].Remove(7);
                                    last--;
                                    player_poker[player_send_t].Visible = false;
                                }
                                if (player_send_n < 7)
                                {
                                    now_card[player_send_f].Insert(0, player_send_n);
                                    all_hand_card[3][player_send_f].Remove(player_send_n);
                                    last--;
                                    player_poker[player_send_t].Visible = false;
                                    if (player_send_f == 0)  //為了他媽的好看一點
                                    {
                                        poker1[player_send_n - 1].Visible = true;
                                        poker1[player_send_n - 1].BringToFront();
                                    }
                                    if (player_send_f == 1)
                                    {
                                        poker2[player_send_n - 1].Visible = true;
                                        poker2[player_send_n - 1].BringToFront();
                                    }
                                    if (player_send_f == 2)
                                    {
                                        poker3[player_send_n - 1].Visible = true;
                                        poker3[player_send_n - 1].BringToFront();
                                    }
                                    if (player_send_f == 3)
                                    {
                                        poker4[player_send_n - 1].Visible = true;
                                        poker4[player_send_n - 1].BringToFront();
                                    }
                                }
                                if (player_send_n > 7)
                                {
                                    now_card[player_send_f].Add(player_send_n);
                                    all_hand_card[3][player_send_f].Remove(player_send_n);
                                    last--;
                                    player_poker[player_send_t].Visible = false;
                                }
                                if (player_send_f == 0) poker1[player_send_n - 1].Visible = true;
                                if (player_send_f == 1) poker2[player_send_n - 1].Visible = true;
                                if (player_send_f == 2) poker3[player_send_n - 1].Visible = true;
                                if (player_send_f == 3) poker4[player_send_n - 1].Visible = true;

                                round++;
                                if (round == 4) round = 0;
                            }
                            else
                            {
                                label_msn.Text = "這張牌不能出";
                                player_send = false;
                            }
                            player_send_f = -1;
                            player_send_n = -1;
                            player_send_t = -1;
                            player_send = false;
                        }
                        else  //蓋牌
                        {
                            all_loss[3] += player_send_n;
                            all_hand_card[3][player_send_f].Remove(player_send_n);
                            last--;
                            player_poker[player_send_t].Visible = false;

                            player_send_f = -1;
                            player_send_n = -1;
                            player_send_t = -1;
                            player_send = false;

                            round++;
                            if (round == 4) round = 0;
                        }
                        

                    }
                }
            }
            else //遊戲結束
            {
                label_last.Text = "剩餘牌數:0";
                label_loss.Text = "蓋牌總數:" + all_loss[3];
                label_ai1_loss.Text = "1號蓋牌:" + all_loss[0];
                label_ai2_loss.Text = "2號蓋牌:" + all_loss[1];
                label_ai3_loss.Text = "3號蓋牌:" + all_loss[2];
                label_player_loss.Text = "玩家蓋牌:" + all_loss[3];
                int winner = 0;
                int t = all_loss[0];
                for (int i = 0; i < 4; i++)
                {
                    if (t >= all_loss[i])
                    {
                        winner = i;
                        t = all_loss[i];
                    }
                }
                if (winner == 3)
                {
                    label_winner.Text = "玩家獲勝";
                }
                else
                {
                    label_winner.Text = (winner + 1) + "號獲勝";
                }
                timer1.Enabled = false;
            }
        }
    }
}
