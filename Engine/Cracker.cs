using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Engine
{
    public class Cracker
    {
        private readonly byte[] _expectedCode;

        public Cracker(string expectedCode)
        {
            _options = new ParallelOptions
            {
                CancellationToken = _cancel.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            _expectedCode = Convert.FromBase64String(expectedCode);
        }

        public delegate void CrackerEventHandler(object sender, CrackerEventArgs e);

        public event CrackerEventHandler? OnCrackerReport;

        private CancellationTokenSource _cancel { set; get; } = new();

        private ICollection<Task> _instances { get; set; } = new List<Task>();

        private ParallelOptions _options { init; get; }

        public void CreateAttempts()
        {
            for (int i = 12; i < 988; i++)
            {
                string _entry = $"{i:000}";

                if (_entry.GroupBy(c => c).Any(g => g.Count() > 1))
                    continue;

                OnCrackerReport?.Invoke(this, new($"Creating attempt: {_entry}"));

                _instances.Add(Task.Run(() =>
                {
                    try
                    {
                        if (!CheckSecretFeature(_entry))
                        {
                            OnCrackerReport?.Invoke(this, new($"Failed attempt: {_entry}", CrackerEventArgs.CrackerEvent.AttemptFailed));
                            return;
                        }

                        OnCrackerReport?.Invoke(this, new($"Found attempt!: {_entry}", CrackerEventArgs.CrackerEvent.AttemptSucceeded));
                        _cancel.Cancel();
                    }
                    catch (Exception ex)
                    {
                        OnCrackerReport?.Invoke(this, new($"Error in attempt: {_entry}", CrackerEventArgs.CrackerEvent.AttemptError));
                        Debug.WriteLine(ex.Message);
                    }
                }));
            }
        }

        public void RunAttempts()
        {
            try
            {
                Parallel.ForEach(_instances, _options, task => task.Wait());
            }
            catch (OperationCanceledException)
            {
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private bool CheckSecretFeature(string attempt)
        {
            using (SHA256 sha256Hash = SHA256.Create())
                return _expectedCode.SequenceEqual(sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(attempt)));
        }
    }
}