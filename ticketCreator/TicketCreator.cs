using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using ZXing.Common;
using ZXing;

/*******************************************************
 * - Class name : TicketCreator
 * - Author : 刘畅   Version : 1.0   Date : 2018/4/15
 * - Description : // 售货小票生成器
 *                 // 输入相关参数后可自动生成小票PDF文件
 * - Operate way：
 * - 1.在调用该类的函数中创建类的实例
 *   TicketCreator tc = new TicketCreator();
 *   
 * - 2.设置店铺及操作人员等身份信息
 *   tc.setOperatorInfo("旗舰店","1001","0018","31230");
 *   
 * - 3.添加商品信息
 *   tc.addGoods("商品1",1,10.23);
 *   
 * - 4.根据店铺需要添加小票备注
 *   tc.addNotes("谢谢惠顾");
 *   
 * - 5.可设置是否生成条码
 *   tc.setCodeOperatorOpened(true);
 *   
 * - 6.开始生成
 *   tc.generatePDF("D:", 50);
 *   
 * 
 * - Function List :
 *  1.setFilePath
 *  2.setWidth
 *  3.setHeight
 *  4.setBorders
 *  5.setFontSize
 *  6.setSeperator
 *  7.setStoreName
 *  8.setTicketID
 *  9.setCasherID
 * 10.setEmployeeID
 * 11.setTime
 * 12.setCard
 * 13.setAlipayID
 * 14.setCodeCreatorOpened
 * 15.getFilePath
 * 16.getStoreName
 * 17.getticketID
 * 18.getcasherID
 * 19.getEmployeeID
 * 20.getTime
 * 21.getDeal
 * 22.getCard
 * 23.getRefund
 * 24.getAliPayID
 * 25.TicketCreator
 * 26.addGoods
 * 27.analyse
 * 28.addNotes
 * 29.generatePDF
 * - Others :
 * //
 * 
 *******************************************************/

namespace ticketCreator
{
    class TicketCreator
    {
        // PDF文件参数
        private string filePath = "";         // 文件保存路径
        private int width = 0;                // PDF宽度
        private int height = 0;               // PDF长度
        private int border_left = 0;          // 左页边距
        private int border_right = 0;         // 右页边距
        private int border_head = 0;          // 上页边距
        private int border_tail = 0;          // 下页边距
        private int fontSize = 0;             // 字体大小
        private string seperator = "";        // 分割线
        private int number_of_seperator = 50; // 分割线的字符数量

        // 小票信息参数
        private string storeName = "";   // 店铺名
        private string ticketID = "";    // 票号
        private string casherID = "";    // 机号
        private string employeeID = "";  // 工号
        private string time = "";        // 出票时间
        private string deal = "";        // 交易金额
        private string card = "";        // 卡券
        private string refund = "";      // 退补
        private string AlipayID = "";    // 支付宝单号
        private string[] gname;          // 商品编号
        private int[] amount;            // 商品数量
        private double[] price;          // 商品价格
        private int overallAmount = 0;   // 总的商品数
        private const int MAX_GOODS = 50;// 最大商品显示数
        private string[] notes;          // 备注
        private int noteAmount = 0;      // 备注数量
        private const int MAX_NOTES = 50;// 最大备注数量

        // 其他支持项
        private bool codeCreatorOpened;  // 开启生成条码

        // set_get函数集
        public void setFilePath(string filePath) { this.filePath = filePath; }
        public void setWidth(int width) { this.width = width; }
        public void setHeight(int height) { this.height = height; }
        public void setBorders(int left, int right, int head, int tail) {
            this.border_left = left;
            this.border_right = right;
            this.border_head = head;
            this.border_tail = tail;
        }
        public void setFontSize(int fontSize) { this.fontSize = fontSize; }
        public void setSeperator(string character) {
            seperator = "";
            for (int i = 0; i < number_of_seperator; i++)
            {
                seperator += character;
            }
        }

        public void setStoreName(string storeName) { this.storeName = storeName; }
        public void setTicketID(string ticketID) { this.ticketID = ticketID; }
        public void setCasherID(string casherID) { this.casherID = casherID; }
        public void setEmployeeID(string employeeID) { this.employeeID = employeeID; }
        public void setTime(string time) { this.time = time; }
        public void setCard(string card) { this.card = card; }
        public void setAlipayID(string AlipayID) { this.AlipayID = AlipayID; }
        public void setCodeCreatorOpened(bool flag) { this.codeCreatorOpened = flag; }

        public string getFilePath() { return filePath; }
        public string getStoreName() { return storeName; }
        public string getticketID() { return ticketID; }
        public string getcasherID() { return casherID; }
        public string getEmployeeID() { return employeeID; }
        public string getTime() { return time; }
        public string getDeal() { return deal; }
        public string getCard() { return card; }
        public string getRefund() { return refund; }
        public string getAliPayID() { return AlipayID; }

        /************************************************
         * Function name : TicketCreator
         * Description : 构造函数
         * Variables : void/string filePath
         ************************************************/
        public TicketCreator()
        {
            // 初始化PDF参数
            seperator = "-------------------------------------------------";
            width = 200;
            height = 800;
            border_left = 1;
            border_right = 1;
            border_head = 1;
            border_tail = 1;
            fontSize = 10;
            gname = new string[MAX_GOODS];
            amount = new int[MAX_GOODS];
            price = new double[MAX_GOODS];
            notes = new string[MAX_NOTES];
            codeCreatorOpened = true;
        }




        /************************************************
         * Function name : addGoods
         * Description : 添加货物到列表
         * Variables : string NAME, int AMOUNT, double PRICE
         ************************************************/
        public void addGoods(string NAME, int AMOUNT, double PRICE)
        {
            gname[overallAmount] = NAME;
            amount[overallAmount] = AMOUNT;
            price[overallAmount] = PRICE;
            overallAmount++;
        }



        /************************************************
         * Function name : analyse
         * Description : 结算
         * Variables : double receive
         ************************************************/
        private void analyse(double receive)
        {
            double sum = 0;
            for (int i = 0; i < overallAmount; i++)
            {
                sum += price[i] * amount[i];
            }
            double sum2 = ((double)((int)(sum * 100))) / 100;
            deal = "" + sum2;
            refund = (receive - sum2) + "";
        }



        /************************************************
        * Function name : addNotes
        * Description : 添加备注
        * Variables : string note
        ************************************************/
        public void addNotes(string note)
        {
            notes[noteAmount] = note;
            noteAmount++;
        }



        /************************************************
         * Function name : generatePDF
         * Description : 生成小票
         * Variables : string filePath
         ************************************************/
        public void generatePDF(string filePath, double receive)
        {
            // 判断路径是否存在
            this.filePath = filePath;
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            // 结算
            analyse(receive);
            
            // 生成条码
            EncodingOptions encodeOption = new EncodingOptions();
            encodeOption.Height = 40;
            encodeOption.Width = 180;
            ZXing.BarcodeWriter wr = new BarcodeWriter();
            wr.Options = encodeOption;
            wr.Format = BarcodeFormat.CODE_39;
            Bitmap img = wr.Write(ticketID);
            img.Save(filePath + "\\" + ticketID + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            // 生成PDF小票
            time = System.DateTime.Now.ToString();
            iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(width, height);
            Document document = new Document(pageSize, border_left, border_right, border_head, border_tail);
            PdfWriter.GetInstance(document, new FileStream(filePath + "\\" + ticketID + ".pdf", FileMode.Create));
            document.Open();
            BaseFont.AddToResourceSearch("iTextAsian.dll");
            BaseFont baseFT = BaseFont.CreateFont("STSong-Light", "UniGB-UCS2-H", BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFT, fontSize);

            document.Add(new Paragraph(seperator, font));
            document.Add(new Paragraph(storeName, font));
            document.Add(new Paragraph("票号：" + ticketID + " 机号：" + casherID + " 工号：" + employeeID, font));
            document.Add(new Paragraph("交易时间：" + time, font));
            document.Add(new Paragraph("合计金额：" + deal + " 卡券：" + card, font));
            document.Add(new Paragraph("据此联购物一个月内开发票！", font));
            document.Add(new Paragraph(seperator, font));
            document.Add(new Paragraph(storeName, font));
            document.Add(new Paragraph("票号：" + ticketID + " 机号：" + casherID + " 工号：" + employeeID, font));
            document.Add(new Paragraph("交易时间：" + time, font));
            document.Add(new Paragraph("*#" + casherID + ticketID + " #*", font));
            document.Add(new Paragraph(seperator, font));
            document.Add(new Paragraph("序号   商品编码（名称）   数量   金额", font));
            string str = "          ";
            for (int i = 0; i < overallAmount; i++)
            {
                document.Add(new Paragraph((i + 1) + str + gname[i] + str + amount[i] + str + price[i], font));
            }
            document.Add(new Paragraph(seperator, font));
            document.Add(new Paragraph("金额： " + deal + " 找零： " + refund, font));
            document.Add(new Paragraph("支付宝单号：" + AlipayID, font));
            if (AlipayID != "")
            {
                document.Add(new Paragraph("支付宝接口：" + deal, font));
            }
            else
            {
                document.Add(new Paragraph("支付宝接口：", font));
            }
            document.Add(new Paragraph(seperator, font));
            for (int i = 0; i < noteAmount; i++)
            {
                document.Add(new Paragraph(notes[i], font));
            }
            document.Add(new Paragraph(seperator, font));
            if (codeCreatorOpened)
            {
                iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(filePath + "\\" + ticketID + ".jpg");
                img2.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                document.Add(img2);
            }
            
            document.Close();
        }



        /************************************************
         * Function name : setOperatorInfo
         * Description : 设置操作员信息
         * Variables : string filePath
         ************************************************/
        public void setOperatorInfo(string STORE, string TICKET, string CASH, string EMPLOYEE)
        {
            storeName = STORE;
            ticketID = TICKET;
            casherID = CASH;
            employeeID = EMPLOYEE;
        }
    }
}
