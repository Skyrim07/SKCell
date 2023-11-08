using UnityEngine;

namespace SKCell
{
    /// <summary>
    /// Random number generator based on PDFs
    /// </summary>
    public class SKRandom
    {
        /// <summary>
        /// Get a random number according to the normal distribution.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="stdDev"></param>
        /// <returns></returns>
        public static float NormalDistribution(float mean = 0, float stdDev = 1)
        {
            float u1 = 1.0f - UnityEngine.Random.value; 
            float u2 = 1.0f - UnityEngine.Random.value;
            float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        /// <summary>
        /// Get a random number according to the Gamma distribution.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        public static float Gamma(int alpha, float beta)
        {
            float gamma = 0f;
            for (int i = 0; i < alpha; i++)
            {
                float u = UnityEngine.Random.value;
                gamma += -Mathf.Log(1 - u) * beta;
            }
            return gamma;
        }

        /// <summary>
        /// Get a random number according to the Beta distribution.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        public static float Beta(float alpha, float beta)
        {
            float gamma1 = Gamma((int)alpha, 1);
            float gamma2 = Gamma((int)beta, 1);
            return gamma1 / (gamma1 + gamma2);
        }

        /// <summary>
        /// Get a random number according to the Cauchy distribution.
        /// </summary>
        /// <param name="x0">Location</param>
        /// <param name="gamma">Scale</param>
        /// <returns></returns>
        public static float Cauchy(float x0, float gamma)
        {
            float u = UnityEngine.Random.value;
            return x0 + gamma * Mathf.Tan(Mathf.PI * (u - 0.5f));
        }

        /// <summary>
        /// Get a random number according to the Logistic distribution.
        /// </summary>
        /// <param name="mu"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float Logistic(float mu, float s)
        {
            float u = UnityEngine.Random.value;
            return mu - s * Mathf.Log(1 / u - 1);
        }

        /// <summary>
        /// Get a random number according to the binomial distribution using normal approximation.
        /// </summary>
        /// <param name="n">Number of trials</param>
        /// <param name="p">Probability of success on each trial</param>
        /// <returns>The number of successes in n trials</returns>
        public static int Binomial(int n, float p)
        {
            if (n > 30 && p > 0.05 && p < 0.95)
            {
                float mean = n * p;
                float stdDev = Mathf.Sqrt(n * p * (1 - p));
                float normalApproximation = NormalDistribution(mean, stdDev);
                int binomialResult = Mathf.RoundToInt(normalApproximation);
                binomialResult = Mathf.Clamp(binomialResult, 0, n);

                return binomialResult;
            }
            else
            {
                int successCount = 0;
                for (int i = 0; i < n; i++)
                {
                    if (UnityEngine.Random.value < p)
                    {
                        successCount++;
                    }
                }
                return successCount;
            }
        }

        /// <summary>
        /// Get a random number according to the uniform distribution.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float Uniform(float from, float to)
        {
            return Random.Range(from, to);
        }

        /// <summary>
        /// Get a random number according to the uniform distribution.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int Uniform(int fromIncl, int toExcl)
        {
            return Random.Range(fromIncl, toExcl);
        }
    }
}
