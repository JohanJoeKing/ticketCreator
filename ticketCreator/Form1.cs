using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ticketCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /******************************************************
         * Function name : button1_Click
         * Description : 生成函数
         * Variables : object sender, EventArgs e
         ******************************************************/
        private void button1_Click(object sender, EventArgs e)
        {
            TicketCreator tc = new TicketCreator();
            tc.setOperatorInfo("益庄店","321304","0018","3123");
            tc.addGoods("雪碧", 2, 2.0);
            tc.addGoods("烤鸭", 1, 15.5);
            tc.addGoods("青岛啤酒灌装", 3, 4.5);
            tc.addNotes("商品质量问题七日内退换");
            tc.addNotes("客服电话：0311-68093454/966118");
            tc.generatePDF("H:\\mySoftware\\PDFTests", 50);
        }
    }
}
