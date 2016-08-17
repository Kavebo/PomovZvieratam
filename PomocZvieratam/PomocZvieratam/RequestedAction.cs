//namespace PomocZvieratam
//{
class RequestedAction
{
    private string typZvierata;
    private string typAkcie;
    public string _typeOfAction
    {
        get
        {
            return typAkcie; ;
        }
        set
        {
            switch (value)
            {
                case "Zber uhynutých zvierat":
                    typAkcie = "Zber";
                    break;
                case "Odchyt zvierat":
                    typAkcie = "Odchyt";
                    break;
                case "Deratizácia":
                    typAkcie = "Deretizacia";
                    break;

                default:
                    typAkcie = "Odchyt";
                    break;
            }
            ;
        }
    }
    public string _typeOfAnimal
    {
        get
        {
            return typZvierata;
        }

        set
        {
            switch (value)
            {
                case "Spoloèenské zvieratá":
                    typZvierata = "Spolocenske Zvierata";
                    break;
                case "Vo¾ne žijúce zvieratá":
                    typZvierata = "Volne zijuce zvierata";
                    break;
                case "Hospodárske zvieratá":
                    typZvierata = "Hospodarske zvierata";
                    break;
                case "Chránené živoèíchy":
                    typZvierata = "Chranene zivocichy";
                    break;
                case "Po¾ovná zver":
                    typZvierata = "Polovna zver";
                    break;
                case "Hlodavce":
                    typZvierata = "Hlodavce";
                    break;
                case "Iné":
                    typZvierata = "Ine";
                    break;

                default:
                    typZvierata = "ani jedno";
                    break;
            }
            ;
        }
    }
    public string _infoAboutAction { get; set; }
    public string _latitude { get; set; }
    public string _logntitude { get; set; }
    public byte[] _imageFile { get; set; }



}
//}