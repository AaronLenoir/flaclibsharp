using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FlacLibSharp.Test
{
    /// <summary>
    /// Tests behaviour with file access rights when saving flac files.
    /// </summary>
    /// <remarks>Related to issue #35 on github: "Save() doesn't create a file with the original permissions"</remarks>
    [TestClass]
    public class FileAccessRightTests
    {
        private string testFile = @"Data\issue35_01.flac";

        [TestInitialize]
        public void Initialize()
        {
            GiveEveryoneReadAccess(testFile);

            Assert.IsTrue(EveryoneHasReadAccess(testFile), $"Test initialization failed, could not give Everyone read access on {testFile}");
        }

        /// <summary>
        /// Reproduces case of issue 35
        /// </summary>
        [TestMethod]
        public void SaveShouldNotClearAccessRights()
        {
            using(var file = new FlacFile(testFile))
            {
                file.Save();
            }

            Assert.IsTrue(EveryoneHasReadAccess(testFile), "Test file lost Everyone Read access after save.");
        }

        /// <summary>
        /// Gives the "Everyone" group read access to a file.
        /// </summary>
        private void GiveEveryoneReadAccess(string path)
        {
            // Reference to "World" or "Everyone" group
            // see: https://msdn.microsoft.com/en-us/library/windows/desktop/aa379649(v=vs.85).aspx
            //      https://stackoverflow.com/questions/7444675/how-can-i-set-a-file-to-be-writeable-by-all-users
            var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            var accessRule = new FileSystemAccessRule(everyone, FileSystemRights.Read, AccessControlType.Allow);

            var fileSecurity = File.GetAccessControl(path);
            fileSecurity.AddAccessRule(accessRule);
            File.SetAccessControl(path, fileSecurity);
        }

        private bool EveryoneHasReadAccess(string path)
        {
            var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            var fileSecurity = File.GetAccessControl(path);
            var acl = fileSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

            foreach(FileSystemAccessRule rule in acl)
            {
                if (rule.IdentityReference.Value == everyone.Value
                    && rule.AccessControlType == AccessControlType.Allow
                    && (rule.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
