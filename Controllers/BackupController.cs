using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using WebUI.Services;

namespace WebUI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly BackupSaver _backupSaver;

        public BackupController(BackupSaver backupSaver)
        {
            _backupSaver = backupSaver;
        }

        [HttpGet]
        public async Task<IEnumerable<Log>> MakeBackup()
        {
            await _backupSaver.MakeBackupsAsync();

            return _backupSaver.Logs;
        }
    }
}

