using SQLite;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using Ji.ClassSR;
using System.Collections.Generic;

namespace Ji.Models
{
    [Table("UserChat")]
    public class UserChat : INotifyPropertyChanged
    {
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [PrimaryKey, AutoIncrement, Column("_DBUserId")]
        public int DBUserId { get; set; }
        //        [PrimaryKey, AutoIncrement, Column("_UserId")]

   
        public int UserId { get; set; }
        [MaxLength(255)]
        public string FIO { get; set; }
        public string Famil
        {
            get
            {
                try
                {
                    return FIO.Split(' ')[0];
                }
                catch (Exception err)
                {
                    
                }
                return FIO;
            }
        }
        [MaxLength(255)]
        public string Skill { get; set; }
        [MaxLength(255)]
        public string OU { get; set; }
        [MaxLength(255)]
        public string Phones { get; set; }
        public int ? PersonalChatId { get; set; }
    
       
        bool _bFavorite;
        public bool bFavorite 
            { 
            get { return _bFavorite; }
            set
            {
                if (_bFavorite != value)
                {
                    _bFavorite = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(bNotFavorite));
                }

            }
        }
        public bool bNotFavorite
        {
            get { return !_bFavorite; }
             set
            {
                if (_bFavorite == value)
                {
                    _bFavorite = !value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(bFavorite));
                }

            }
        }


        [MaxLength(25)]
        public DateTime Version { get; set; }

        private bool b_ImageDefault = false;
        private bool b_Selected;
        public bool Selected
        {
            get { return b_Selected; }
            set { b_Selected = value; }
        }
        [Ignore]
        public string ImageURL
        {
            get
            {
                // https://doumer.me/resolve-image-loading-error-in-xamarin-forms/
                // https://github.com/xamarin/Xamarin.Forms/issues/7248

                //              string url = "http://194.190.100.194/2.jpg";
                string url = "http://194.190.100.194/xml/GetUserImage.ashx?id=" + UserId + "&tiket=" + App.ddd.connectInterface.TokenSeanceId;
                return url;
            }
            

        }
       

        Image _ImageFoto = null;

        private  ImageSource ImageFoto
        {

            get
            {
                if (_ImageFoto == null)
                {
                    /* //http://localhost/xml/GetUserImage.ashx?id=4
                     //http::/194.190.100.194/xml/GetUserImage.ashx?id=4
                     //var t = new Image { Source = "user46.png"};
                     string url = ImageURL;
                     _ImageFoto = new Image { Source = ImageSource.FromUri(new System.Uri(url)) };

                    // _ImageFoto = t;
                     b_ImageDefault = true;


                     var image = new Image
                     {
                         Source = ImageSource.FromUri(new System.Uri(url))
                     };


                     StreamImageSource i = new StreamImageSource();
                */


                    //if (image != null)
                    //{
                    //    byte[] b = image.Content;
                    //    Stream ms = new MemoryStream(b);
                    //    image1.Source = ImageSource.FromStream(() => ms);
                    //}
                    try
                    {
                        //UriImageSource sss =
                        //new UriImageSource
                        //{
                        //    Uri = new Uri(ImageURL),//ImageURL
                        //CachingEnabled = false,
                        //    CacheValidity = new TimeSpan(5, 0, 0, 0)
                        //};
                        return null;
                    }catch (Exception err)
                    {

                    }
                    return null;
                    //var image = new Image { Source = new System.Uri(ImageURL) };
                    
                    //      ImageSource isFoto = ImageSource.FromUri(new System.Uri(ImageURL));
                    //return isFoto;
                }
                return null;
            }
            set { b_ImageDefault = false; /* _ImageFoto = value; */ }
        }

        [Ignore]
        public string[] Params { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [Ignore]
        public System.Collections.Generic.List<XParam> GetParamsList { get
            {
                if (Params != null)
                {
                    System.Collections.Generic.List<XParam> p = new System.Collections.Generic.List<XParam>();
                    foreach (string s in Params)
                    {
                        if (!s.Contains(" Парковка"))
                        {
                            if (!String.IsNullOrEmpty(s))
                            {
                                XParam p1 = new XParam(s);
                                p.Add(p1);
                            }
                        }
                        else
                        {
                        }
                    }


                    return p;// string.Join("\n", Params);
                }
                else
                    return null;
            }
            set
            {
                //if (value!=null)
                //Params =  value.Split('\n').ToArray();
                //else
                //{

                //}
            }
        }
        [Ignore]
        public string GetMobilePhone { get
            {
                try
                {
                    string[] phones = Params.Where(s => s!=null && s.StartsWith("Телефон пользователя\t")).ToArray();
                    foreach (string s in phones)
                    {
                        string phone = s.Substring("Телефон пользователя\t".Length);
                        string[] data = phone.Split(',');
                        foreach (string p in data)
                        {
                            if (p.Trim().StartsWith("+"))
                            {
                                //Regex regex = new Regex(@"/[^+\d]/g");
                                //  string result = regex.Replace(p.Trim(),'');
                                string result = Regex.Replace(p.Trim(), @"\D+", "", RegexOptions.ECMAScript);
                                if (result.Length>=10)
                                return result;
                            }
                        }
                    }
                }catch (Exception err)
                {

                }
                return "";
                    //string.Join(",", 

              
            }
        }

        public List<UserPosition> UserPositions { get; set; } 
        public string OUMini { get; set; }
    }
}
