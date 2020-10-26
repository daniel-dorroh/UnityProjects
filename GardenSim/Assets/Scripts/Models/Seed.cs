namespace Models
{
    public class Seed<T>
        where T : ILife, new()
    {
        private Genes _genes;
        private bool _isSprouted = false;
        private ILife _life = null;

        public Seed(Genes genes)
        {
            _genes = genes;
        }

        public bool IsSprouted => _isSprouted;
        public ILife Life => _life;

        public T Sprout()
        {
            var life = new T();
            life.Init(_genes);
            _isSprouted = true;
            _life = life;
            return life;
        }
    }
}
