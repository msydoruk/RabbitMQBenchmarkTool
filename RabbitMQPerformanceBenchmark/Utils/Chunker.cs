namespace RabbitMQPerformanceBenchmark.Utils
{
    public static class Chunker
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> input, int size)
        {
            var chunk = new List<T>(size);
            foreach (T item in input)
            {
                chunk.Add(item);
                if (chunk.Count == size)
                {
                    yield return chunk;
                    chunk = new List<T>(size);
                }
            }

            if (chunk.Count > 0)
                yield return chunk;
        }
    }
}
