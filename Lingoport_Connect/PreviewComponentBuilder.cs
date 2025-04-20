using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Lingoport.LocalyzerConnect.Preview
{
    public class PreviewComponentBuilder : IFileTypeComponentBuilder
    {
        public PreviewComponentBuilder()
        {
            DebugLogger.Log(">>> PreviewComponentBuilder: Constructor called");
        }

        public IFileTypeManager FileTypeManager { get; set; }

        private IFileTypeDefinition _fileTypeDefinition;
        public IFileTypeDefinition FileTypeDefinition
        {
            get => _fileTypeDefinition;
            set
            {
                _fileTypeDefinition = value;
                DebugLogger.Log(">>> PreviewComponentBuilder: FileTypeDefinition set");
            }
        }

        public IFileTypeInformation BuildFileTypeInformation(string name)
        {
            DebugLogger.Log($">>> PreviewComponentBuilder: BuildFileTypeInformation called with name={name}");
            return null;
        }

        public IQuickTagsFactory BuildQuickTagsFactory(string name)
        {
            DebugLogger.Log($">>> PreviewComponentBuilder: BuildQuickTagsFactory called with name={name}");
            return null;
        }

        public IAbstractGenerator BuildAbstractGenerator(string name)
        {
            DebugLogger.Log($">>> PreviewComponentBuilder: BuildAbstractGenerator called with name={name}");
            if (name == "Generator_RealTimePreview")
            {
                DebugLogger.Log(">>> Creating InternalPreviewWriter...");
                return FileTypeManager.BuildFileGenerator(
                    FileTypeManager.BuildNativeGenerator(new InternalPreviewWriter())
                );
            }
            return null;
        }

        public IAbstractPreviewControl BuildPreviewControl(string name)
        {
            DebugLogger.Log($">>> PreviewComponentBuilder: BuildPreviewControl called with name={name}");
            try
            {
                if (name == "PreviewControl_InternalNavigablePreview")
                {
                    DebugLogger.Log(">>> Returning InternalPreviewController");
                    return new InternalPreviewController();
                }

                DebugLogger.Log(">>> Returning null for unknown preview control name");
                return null;
            }
            catch (Exception ex)
            {
                DebugLogger.Log(">>> Exception in BuildPreviewControl: " + ex);
                throw;
            }
        }

        public IPreviewSetsFactory BuildPreviewSetsFactory(string name)
        {
            DebugLogger.Log($">>> PreviewComponentBuilder: BuildPreviewSetsFactory called with name={name}");
            return null;
        }

        public IAbstractPreviewApplication BuildPreviewApplication(string name)
        {
            DebugLogger.Log($">>> PreviewComponentBuilder: BuildPreviewApplication called with name={name}");
            return null;
        }

        public IAdditionalGeneratorsInfo BuildAdditionalGeneratorsInfo(string name)
        {
            DebugLogger.Log($">>> PreviewComponentBuilder: BuildAdditionalGeneratorsInfo called with name={name}");
            return null;
        }

        public IVerifierCollection BuildVerifierCollection(string name) => null;
        public IFileExtractor BuildFileExtractor(string name) => null;
        public IFileGenerator BuildFileGenerator(string name) => null;
        public INativeFileParser BuildNativeFileParser(string name) => null;
        public IAbstractPreviewController BuildPreviewController(string name) => null;
        public IBilingualParser BuildBilingualParser(string name) => null;
        public IBilingualDocumentGenerator BuildBilingualGenerator(string name) => null;
        public INativeGenerator BuildNativeGenerator(string name) => null;
        public INativeFileSniffer BuildFileSniffer(string name) => null;
    }
}
