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
                case "Zber uhynut�ch zvierat":
                    typAkcie = "Zber";
                    break;
                case "Odchyt zvierat":
                    typAkcie = "Odchyt";
                    break;
                case "Deratiz�cia":
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
                case "Spolo�ensk� zvierat�":
                    typZvierata = "Spolocenske Zvierata";
                    break;
                case "Vo�ne �ij�ce zvierat�":
                    typZvierata = "Volne zijuce zvierata";
                    break;
                case "Hospod�rske zvierat�":
                    typZvierata = "Hospodarske zvierata";
                    break;
                case "Chr�nen� �ivo��chy":
                    typZvierata = "Chranene zivocichy";
                    break;
                case "Po�ovn� zver":
                    typZvierata = "Polovna zver";
                    break;
                case "Hlodavce":
                    typZvierata = "Hlodavce";
                    break;
                case "In�":
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