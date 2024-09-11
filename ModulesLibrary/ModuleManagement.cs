using System.Reflection;

namespace ModulesLibrary
{
    public class ModuleManagement
    {
        public static int SelfStudyCalc(int credits, int weeks, int classHrs)
        {
            int selfStudyHrs = (credits * 10 / weeks) - classHrs;
            return selfStudyHrs;
        }

    }

}
