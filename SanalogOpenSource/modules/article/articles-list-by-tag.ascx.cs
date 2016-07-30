﻿using Snlg_DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class modules_article_articles_list_by_tag : Snlg_UserControlBaseClass
{
    //bir sayfada kaç kayıt gösterilecek
    protected byte _PageSize = 2;
    public byte PageSize
    {
        get { return _PageSize; }
        set { _PageSize = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request.QueryString["Tagvalue"]))
        {
            Int16 currentPage = 1;
            try { currentPage = Convert.ToInt16(Request.RawUrl.Substring(Request.RawUrl.IndexOf("page=") + 5).Trim()); }
            catch { }

            //makaleler db'den çekilyior.
            #region parametreler
            Snlg_DBParameter[] DBPrms = new Snlg_DBParameter[5];
            DBPrms[0] = new Snlg_DBParameter("@KayitSayisi", SqlDbType.Int, -1, ParameterDirection.InputOutput);
            DBPrms[1] = new Snlg_DBParameter("@Dil", SqlDbType.SmallInt, Snlg_ConfigValues.s_ZDilId);
            DBPrms[2] = new Snlg_DBParameter("@TagValues", SqlDbType.NVarChar, Request.QueryString["Tagvalue"].ToString());
            DBPrms[3] = new Snlg_DBParameter("@CurrentPage", SqlDbType.Int, currentPage);
            DBPrms[4] = new Snlg_DBParameter("@PageSize", SqlDbType.Int, PageSize);
            #endregion

            DataTable Dt = vt.DataTableOlustur("snlg_V1.zsp_Makale_TagMakaleListesi", CommandType.StoredProcedure, DBPrms);
            if (Dt.Rows.Count > 0)
            {
                base.pg.ScriptOrCssFileImportHead("/scripts/jquery.raty-2.4.5/js/jquery.raty.min.js");
                Rpt.DataSource = Dt;
                Rpt.DataBind();

            }

            string tagText = "";
            foreach (string item in Request.QueryString["tagvalue"].Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                tagText += item[0].ToString().ToUpper() + item.Substring(1) + " ";

            Page.Title = tagText;
            LtrMakaleTagAi.Text = tagText;
            if (this.Visible)
            {//eğer bu kontrol visible ise sayfalamayı hazırla
             //kayıtlı duyuru sayısı alınıyor
                Int16 kayitSay = Convert.ToInt16(DBPrms[0].Deger);
                #region sayfalama
                if (kayitSay > PageSize)
                {//eğer kayıt sayısı sayfada gösterilecek olan kayıt sayısından büyükse sayfalama yap (demekki 1'den fazla sayfa var)
                 //listelenecek sayfa sayısı hesaplanıyor.

                    int rakam = 0;
                    Int16 sayfaSay = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(kayitSay) / PageSize));
                    LtrSayfalama.Text = "<ul class='pagination'>";
                    LtrSayfalama.Text += string.Format("<li><a  class='next' href=\"?page={0}\">←</a></li>", ((currentPage - 1) > 0 ? (currentPage - 1) : 1).ToString());

                    for (Int16 i = 1; i <= sayfaSay; i++)
                    {
                        if (i == currentPage)//page null ise ilk sayfayı curent yap
                            LtrSayfalama.Text += string.Format("<li class='active'><a  href=\"?page={0}\">{0}</a></li>", i.ToString());
                        else//diğer sayfaları normal link yap (current yapma)
                            LtrSayfalama.Text += string.Format("<li><a href=\"?page={0}\">{0}</a></li>", i.ToString());

                        rakam = i;
                    }
                    LtrSayfalama.Text += string.Format("<li><a  class='next' href=\"?page={0}\">→</a></li>", (((currentPage + 1) <= sayfaSay) ? currentPage + 1 : currentPage).ToString());
                    LtrSayfalama.Text += "</ul>";
                }
                else//eğer sayfalanacak kadar kayıt yoksa sayfalama kısmını gizle
                    LtrSayfalama.Visible = false;
                #endregion
            }
        }
    }

    public string MakaleKategoriListesi(string MakaleID)
    {
        string Kategoriler = string.Empty;
        Snlg_DBParameter[] DBPrms = new Snlg_DBParameter[2];
        DBPrms[0] = new Snlg_DBParameter("@MakId", SqlDbType.Int, int.Parse(Eval("MakId").ToString()));
        DBPrms[1] = new Snlg_DBParameter("@Dil", SqlDbType.SmallInt, Snlg_ConfigValues.s_ZDilId);

        DataTable Dt = vt.DataTableOlustur("snlg_V1.zsp_Makale_KategoriListesi", CommandType.StoredProcedure, DBPrms);

        if (Dt.Rows.Count > 0)
        {
            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                Kategoriler = " <a href=\"/" + Snlg_ConfigValues.s_Dil + "/" + Snlg_ConfigValues.GetUrlValueByKey("blog") + "/" + Dt.Rows[i]["SeoUrl"].ToString() + Snlg_ConfigValues.urlExtension + "\">" + Dt.Rows[i]["KtgAd"].ToString() + "</a>";
            }
        }
        return Kategoriler;

    }
}