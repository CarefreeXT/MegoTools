using Caredev.MegoTools.ViewModel;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Runtime.Versioning;
using EnvDTE80;
using System.Threading;
using Res = Caredev.MegoTools.Properties.Resources;

namespace Caredev.MegoTools
{
    public class VSMegoWizard : IVsTemplateWizard
    {
        #region IWizard
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return _AllowAddProjectItem;
        }
        #endregion

        private bool _AllowAddProjectItem = false;

        private void InstallNugetPackage(Project project, string packageId = "Caredev.Mego")
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            var packageInstaller = componentModel.GetService<IVsPackageInstaller2>();
            var installerServices = componentModel.GetService<IVsPackageInstallerServices>();
            if (installerServices != null)
            {
                if (!installerServices.IsPackageInstalled(project, packageId))
                {
                    packageInstaller.InstallPackagesFromVSExtensionRepository(
                        "Caredev.MegoTools.df8930e0-ffc0-45d6-87d1-d87791f4a7f9", false, false, project, new Dictionary<string, string>
                        {
                            { packageId, "1.1.0"}
                        });
                    //packageInstaller.InstallLatestPackage(null, project, packageId, false, false);
                }
            }
        }

        private void WriteConnectionString(NewItemWizardViewModel vm, ProjectItem item)
        {
            var filename = item.FileNames[0];
            var doc = new XmlDocument();
            doc.Load(filename);
            var root = doc.DocumentElement["connectionStrings"];
            if (root == null)
            {
                root = doc.CreateElement("connectionStrings");
                doc.DocumentElement.AppendChild(root);
            }
            var con = doc.ChildNodes.OfType<XmlElement>()
                .Where(a => a.Attributes["name"] != null && a.Attributes["name"].Value == vm.ConfigName)
                .FirstOrDefault();
            if (con == null)
            {
                con = doc.CreateElement("add");
                con.SetAttribute("name", vm.ConfigName);
                con.SetAttribute("connectionString", vm.Information.ConnectionString);
                con.SetAttribute("providerName", vm.Information.ProviderName);
                root.AppendChild(con);
            }
            doc.Save(filename);
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            bool iscancel = false;
            try
            {
                var dte = (DTE)automationObject;
                var currentProject = dte.ActiveSolutionProjects[0] as Project;
                var kind = currentProject.Kind;

                var framework = GetFrameworkName(currentProject);
                string language = string.Empty;
                var isnetcore = false;
                var val = currentProject.Kind.ToUpper().Trim('{', '}');
                switch (currentProject.Kind.ToUpper().Trim('{', '}'))
                {
                    case "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"://.net framework
                        language = "CSharp";
                        break;
                    case "F184B08F-C81C-45F6-A57F-5ABD9991F28F"://.net framework
                        language = "VisualBasic";
                        break;
                    case "9A19103F-16F7-4668-BE54-9A1E7A4F7556"://.net core
                        language = "CSharp";
                        isnetcore = true;
                        break;
                    case "778DAE3C-4631-46EA-AA77-85C1314464D9"://.net core
                        language = "VisualBasic";
                        isnetcore = true;
                        break;
                    default:
                        throw new NotSupportedException(currentProject.Kind);
                }
                var iswebproject = false;
                if (!isnetcore)
                {
                    var extenderNames = (string[])currentProject.ExtenderNames;
                    iswebproject = extenderNames.Contains("WebApplication");
                }
                else
                {
                    replacementsDictionary.Add("$netcore$", "true");
                }
                var configFileName = iswebproject ? "Web.config" : "App.config";
                var templateName = iswebproject ? "WebConfig.zip" :
                    (language == "VisualBasic" ? "AppConfiguration.zip" : "AppConfig.zip");

                NewItemWizardViewModel vm = new NewItemWizardViewModel()
                {
                    ConfigName = replacementsDictionary["$safeitemname$"],
                    Language = language
                };
                if (DialogHelper.ShowDialog(ref vm) == true)
                {
                    if (vm.IsSaveConfig && !isnetcore)
                    {
                        var configureItem = FindProjectItemConfigure(currentProject, configFileName);
                        if (configureItem == null)
                        {
                            var templatePath = ((Solution2)dte.Solution).GetProjectItemTemplate(templateName, language);
                            currentProject.ProjectItems.AddFromTemplate(templatePath, configFileName);
                            configureItem = FindProjectItemConfigure(currentProject, configFileName);
                        }
                        replacementsDictionary.Add("$megoargu$", $"\"{vm.ConfigName}\"");
                        WriteConnectionString(vm, configureItem);
                    }
                    else
                    {
                        var info = vm.Information;
                        replacementsDictionary.Add("$megoargu$", $"\"{info.ConnectionString}\", \"{info.ProviderName}\"");
                    }
                    var generator = vm.Generator;
                    replacementsDictionary.Add("$megodbset$", generator.GenerateDbSet());
                    replacementsDictionary.Add("$megoclassdef$", generator.GenerateSetClass());
                    _AllowAddProjectItem = true;
                    InstallNugetPackage(currentProject);
                }
                else
                {
                    iscancel = true;
                }
            }
            catch (Exception ex)
            {
                DialogHelper.MessageBox(ex.Message);
            }
            if (iscancel)
            {
                throw new WizardCancelledException("The wizard has been cancelled by the user.");
            }
        }
        
        private static ProjectItem FindProjectItemConfigure(Project currentProject, string filename)
        {
            filename = filename.ToLower();
            ProjectItem configureItem = null;
            foreach (var item in currentProject.ProjectItems.OfType<ProjectItem>())
            {
                var name = item.Name.ToLower();
                if (name == filename)
                    if (name == filename)
                    {
                        configureItem = item;
                        break;
                    }
            }

            return configureItem;
        }

        private static FrameworkName GetFrameworkName(Project currentProject)
        {
            foreach (var property in currentProject.Properties.OfType<Property>())
            {
                if (property.Name == "TargetFrameworkMoniker")
                {
                    return new FrameworkName((string)property.Value);
                }
            }
            return null;
        }


    }
}
