using Microsoft.Extensions.Options;
using NCrontab;
using PropertyReservationWeb.Service.Interfaces;

namespace PropertyReservationWeb.WorkerUpdateService
{
    public class DailyUpdateWorker
    {
        private readonly ILogger<DailyUpdateWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CrontabSchedule _schedule;

        public DailyUpdateWorker(IServiceScopeFactory serviceScopeFactory, ILogger<DailyUpdateWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            try
            {
                _schedule = CrontabSchedule.Parse("0 3 * * *");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Некорректное значение CronExpression в конфигурации. Используется значение по умолчанию (0 3 * * *).");
                _schedule = CrontabSchedule.Parse("0 3 * * *");
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Сервис обновления паспортов запущен: {time}", DateTime.Now);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var now = DateTime.Now;
                    var nextRunTime = _schedule.GetNextOccurrence(now);
                    _logger.LogInformation("Следующий запуск обновления паспортов: {nextRunTime}", nextRunTime);

                    var delay = nextRunTime - now;

                    await Task.Delay(delay);

                    using var scope = _serviceScopeFactory.CreateScope();
                    var rentalService = scope.ServiceProvider.GetRequiredService<IRentalRequestService>();

                    _logger.LogInformation("Начало обновления паспортов: {time}", DateTime.Now);
                    var response = await rentalService.UpdateRentalStatusComplete();
                    _logger.LogInformation("Обновление завершено: {time}. Результат: {description}", DateTime.Now, response.Description);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Обновление паспортов отменено.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обновлении паспортов.");
                }
            }
        }
    }
}
