// ***********************************************************************
// Copyright (c) 2016-2018 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace TestCentric.Gui.Presenters.Main
{
    using Elements;
    using Views;

    public class CommandTests : MainPresenterTestBase
    {
        private static string[] NO_FILES_SELECTED = new string[0];
        private static string NO_FILE_PATH = null;

        // TODO: Because the presenter opens dialogs for these commands,
        // they can't be tested directly. This could be fixed if the
        // presenter asked the view to open dialogs.

        //[Test]
        //public void NewProjectCommand_CallsNewProject()
        //{
        //    View.NewProjectCommand.Execute += Raise.Event<CommandHandler>();
        //    // This is NYI, change when we implement it
        //    Model.DidNotReceive().NewProject();
        //}

        [TestCase(false, false, "Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*")]
        [TestCase(true, false, "Projects & Assemblies (*.nunit,*.dll,*.exe)|*.nunit;*.dll;*.exe|NUnit Projects (*.nunit)|*.nunit|Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*")]
        [TestCase(false, true, "Projects & Assemblies (*.csproj,*.fsproj,*.vbproj,*.vjsproj,*.vcproj,*.sln,*.dll,*.exe)|*.csproj;*.fsproj;*.vbproj;*.vjsproj;*.vcproj;*.sln;*.dll;*.exe|Visual Studio Projects (*.csproj,*.fsproj,*.vbproj,*.vjsproj,*.vcproj,*.sln)|*.csproj;*.fsproj;*.vbproj;*.vjsproj;*.vcproj;*.sln|Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*")]
        [TestCase(true, true, "Projects & Assemblies (*.nunit,*.csproj,*.fsproj,*.vbproj,*.vjsproj,*.vcproj,*.sln,*.dll,*.exe)|*.nunit;*.csproj;*.fsproj;*.vbproj;*.vjsproj;*.vcproj;*.sln;*.dll;*.exe|NUnit Projects (*.nunit)|*.nunit|Visual Studio Projects (*.csproj,*.fsproj,*.vbproj,*.vjsproj,*.vcproj,*.sln)|*.csproj;*.fsproj;*.vbproj;*.vjsproj;*.vcproj;*.sln|Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*")]
        public void OpenCommand_DisplaysDialogCorrectly(bool nunitSupport, bool vsSupport, string filter)
        {
            // Return no files so model is not called
            _view.DialogManager.SelectMultipleFiles(null, null).ReturnsForAnyArgs(NO_FILES_SELECTED);
            _model.NUnitProjectSupport.Returns(nunitSupport);
            _model.VisualStudioSupport.Returns(vsSupport);

            _view.OpenCommand.Execute += Raise.Event<CommandHandler>();

            _view.DialogManager.Received().SelectMultipleFiles("Open Project", filter);
        }

        [Test]
        public void OpenCommand_FileSelected_LoadsTests()
        {
            var files = new string[] { Path.GetFullPath("/path/to/test.dll") };
            _view.DialogManager.SelectMultipleFiles(null, null).ReturnsForAnyArgs(files);

            _view.OpenCommand.Execute += Raise.Event<CommandHandler>();

            _model.Received().LoadTests(files);
        }

        [Test]
        public void OpenCommand_NoFileSelected_DoesNotLoadTests()
        {
            _view.DialogManager.SelectMultipleFiles(null, null).ReturnsForAnyArgs(NO_FILES_SELECTED);

            _view.OpenCommand.Execute += Raise.Event<CommandHandler>();

            _model.DidNotReceiveWithAnyArgs().LoadTests(null);
        }

        [TestCase(false, false, "Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*")]
        [TestCase(true, false, "Projects & Assemblies (*.nunit,*.dll,*.exe)|*.nunit;*.dll;*.exe|NUnit Projects (*.nunit)|*.nunit|Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*")]
        [TestCase(false, true, "Projects & Assemblies (*.csproj,*.fsproj,*.vbproj,*.vjsproj,*.vcproj,*.sln,*.dll,*.exe)|*.csproj;*.fsproj;*.vbproj;*.vjsproj;*.vcproj;*.sln;*.dll;*.exe|Visual Studio Projects (*.csproj,*.fsproj,*.vbproj,*.vjsproj,*.vcproj,*.sln)|*.csproj;*.fsproj;*.vbproj;*.vjsproj;*.vcproj;*.sln|Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*")]
        [TestCase(true, true, "Projects & Assemblies (*.nunit,*.csproj,*.fsproj,*.vbproj,*.vjsproj,*.vcproj,*.sln,*.dll,*.exe)|*.nunit;*.csproj;*.fsproj;*.vbproj;*.vjsproj;*.vcproj;*.sln;*.dll;*.exe|NUnit Projects (*.nunit)|*.nunit|Visual Studio Projects (*.csproj,*.fsproj,*.vbproj,*.vjsproj,*.vcproj,*.sln)|*.csproj;*.fsproj;*.vbproj;*.vjsproj;*.vcproj;*.sln|Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*")]
        public void AddTestFilesCommand_DisplaysDialogCorrectly(bool nunitSupport, bool vsSupport, string filter)
        {
            // Return no files so model is not called
            _view.DialogManager.SelectMultipleFiles(null, null).ReturnsForAnyArgs(NO_FILES_SELECTED);
            _model.NUnitProjectSupport.Returns(nunitSupport);
            _model.VisualStudioSupport.Returns(vsSupport);

            _view.AddTestFilesCommand.Execute += Raise.Event<CommandHandler>();

            _view.DialogManager.Received().SelectMultipleFiles("Add Test Files", filter);
        }

        [Test]
        public void AddTestFilesCommand_TellsModelToLoadTests()
        {
            var testFiles = new List<string>();
            testFiles.Add("FILE1");
            testFiles.Add("FILE2");
            _model.TestFiles.Returns(testFiles);

            var filesToAdd = new string[] { Path.GetFullPath("/path/to/test.dll") };
            _view.DialogManager.SelectMultipleFiles(null, null).ReturnsForAnyArgs(filesToAdd);

            var allFiles = new List<string>(testFiles);
            allFiles.AddRange(filesToAdd);

            _view.AddTestFilesCommand.Execute += Raise.Event<CommandHandler>();

            _model.Received().LoadTests(Arg.Is<List<string>>(l => l.SequenceEqual(allFiles)));
        }

        [Test]
        public void AddTestFilesCommand_WhenNothingIsSelected_DoesNotLoadTests()
        {
            _view.DialogManager.SelectMultipleFiles(null, null).ReturnsForAnyArgs(NO_FILES_SELECTED);

            _view.AddTestFilesCommand.Execute += Raise.Event<CommandHandler>();

            _model.DidNotReceive().LoadTests(Arg.Any<IList<string>>());
        }

        [Test]
        public void CloseCommand_CallsUnloadTest()
        {
            _view.CloseCommand.Execute += Raise.Event<CommandHandler>();
            _model.Received().UnloadTests();
        }

        //[Test]
        //public void SaveCommand_CallsSaveProject()
        //{
        //    _view.SaveCommand.Execute += Raise.Event<CommandHandler>();
        //    _model.Received().SaveProject();
        //}

        //[Test]
        //public void SaveAsCommand_CallsSaveProject()
        //{
        //    View.SaveAsCommand.Execute += Raise.Event<CommandHandler>();
        //    // This is NYI, change when we implement it
        //    Model.DidNotReceive().SaveProject();
        //}

        [Test]
        public void SaveResultsCommand_DisplaysDialogCorrectly()
        {
            // Return no file path so model is not called
            _view.DialogManager.GetFileSavePath(null, null).ReturnsForAnyArgs(NO_FILE_PATH);

            _view.SaveResultsCommand.Execute += Raise.Event<CommandHandler>();

            _view.DialogManager.Received().GetFileSavePath("Save Results as XML", "XML Files (*.xml)|*.xml|All Files (*.*)|*.*");
        }

        [Test]
        public void SaveResultsCommand_FilePathSelected_SavesResults()
        {
            var savePath = Path.GetFullPath("/path/to/TestResult.xml");
            _view.DialogManager.GetFileSavePath(null, null).ReturnsForAnyArgs(savePath);

            _view.SaveResultsCommand.Execute += Raise.Event<CommandHandler>();

            _model.Received().SaveResults(savePath);
        }

        [Test]
        public void SaveResultsCommand_NoFilePathSelected_DoesNotSaveResults()
        {
            _view.DialogManager.GetFileSavePath(null, null).ReturnsForAnyArgs(NO_FILE_PATH);

            _view.SaveResultsCommand.Execute += Raise.Event<CommandHandler>();

            _model.DidNotReceiveWithAnyArgs().SaveResults(null);
        }

        [Test]
        public void ReloadTestsCommand_CallsReloadTests()
        {
            _view.ReloadTestsCommand.Execute += Raise.Event<CommandHandler>();
            _model.Received().ReloadTests();
        }

        public void SelectRuntimeCommand_PopsUpMenu()
        {
        }

        public void RecentProjectsMenu_PopsUpMenu()
        {
        }

        public void ExitCommand_CallsExit()
        {
        }

        [Test]
        public void ChangeFontCommand_DisplaysFontDialog()
        {
            Font currentFont = _settings.Gui.Font = new Font(FontFamily.GenericSansSerif, 12.0f);
            // Return same font to avoid setting the font
            _view.DialogManager.SelectFont(null).ReturnsForAnyArgs(currentFont);

            _view.ChangeFontCommand.Execute += Raise.Event<CommandHandler>();

            _view.DialogManager.Received().SelectFont(currentFont);
        }

        [Test]
        public void ChangeFontCommand_ChangesTheFont()
        {
            Font currentFont = _settings.Gui.Font = new Font(FontFamily.GenericSansSerif, 12.0f);
            Font newFont = new Font(FontFamily.GenericSerif, 16.0f);

            _view.DialogManager.SelectFont(null).ReturnsForAnyArgs(newFont);

            _view.ChangeFontCommand.Execute += Raise.Event<CommandHandler>();

            _view.Received().Font = newFont;
            Assert.That(_settings.Gui.Font, Is.EqualTo(newFont));
        }

        [Test]
        public void ApplyFontEvent_ChangesTheFont()
        {
            Font currentFont = _settings.Gui.Font = new Font(FontFamily.GenericSansSerif, 12.0f);
            Font newFont = new Font(FontFamily.GenericSerif, 16.0f);

            _view.DialogManager.ApplyFont += Raise.Event<ApplyFontHandler>(newFont);

            _view.Received().Font = newFont;
            Assert.That(_settings.Gui.Font, Is.EqualTo(newFont));
        }
    }
}
