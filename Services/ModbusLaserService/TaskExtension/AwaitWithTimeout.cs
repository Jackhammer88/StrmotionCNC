using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModbusLaserService.TaskExtension
{
    public static class AwaitWithTimeoutExtension
    {
        public static async Task AwaitWithTimeout(this Task task, int timeout, Action success, Action error)
        {
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                success();
            }
            else
            {
                error();
            }
        }
    }
}
