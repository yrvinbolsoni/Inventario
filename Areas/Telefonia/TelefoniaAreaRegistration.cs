using System.Web.Mvc;

namespace IntraGriegHomolog.Areas.Telefonia
{
    public class TelefoniaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Telefonia";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Telefonia_default",
                "Telefonia/{controller}/{action}/{id}",
                new { controller = "Inicio" , action = "Index", id = UrlParameter.Optional }
            );
        }
    }

     
}