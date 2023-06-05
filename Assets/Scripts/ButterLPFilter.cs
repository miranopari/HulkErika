using UnityEngine;

// NOTE on resonance parameter:
// https://modwiggler.com/forum/viewtopic.php?t=150524#:~:text=Resonance%20is%20an%20additional%20controlled,self%2Doscillation%20of%20the%20filter.

/// <summary>
/// Butterworth low pass filter
/// </summary>
public class ButterLPFilter
{
    private int m_inputLen;
    public float m_resonance;
    public float m_cutoffFreq;
    private float[] m_in1;
    private float[] m_in2;
    private float[] m_out1;
    private float[] m_out2;
    private float[] m_output;

    public float[] Output
    {
        get { return m_output; }
    }


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="resonance">Resonance amount, from sqrt(2) to ~ 0.1</param>
    /// <param name="cutoffFreq">Cutoff frequency (from ~0Hz to SampleRate / 2 - though many synths seem to filter only up to SampleRate/4)</param>
    public ButterLPFilter(int inputLen, float resonance, float cutoffFreq)
    {
        Debug.Assert(inputLen != 0 || inputLen > 0);

        m_inputLen = inputLen;
        m_resonance = resonance;
        m_cutoffFreq = cutoffFreq;

        m_in1 = new float[inputLen];
        m_in2 = new float[inputLen];
        m_out1 = new float[inputLen];
        m_out2 = new float[inputLen];
        m_output = new float[inputLen];

        for (int i = 0; i < inputLen; i++)
        {
            m_in1[i] = 0;
            m_in2[i] = 0;
            m_out1[i] = 0;
            m_out2[i] = 0;
            m_output[i] = 0;
        }
    }

    public float[] Filter(float[] input, float sampleRate)
    {
        Debug.Assert(input.Length == m_inputLen);

        float c = 1.0f / Mathf.Tan(Mathf.PI * m_cutoffFreq / sampleRate);

        float a1 = 1.0f / (1.0f + m_resonance * c + c * c);
        float a2 = 2.0f * a1;
        float a3 = a1;
        float b1 = 2.0f * (1.0f - c * c) * a1;
        float b2 = (1.0f - m_resonance * c + c * c) * a1;

        for (int i = 0; i < input.Length; i++)
        {
            m_output[i] = (a1 * input[i]) +
                (a2 * m_in1[i]) +
                (a3 * m_in2[i]) -
                (b1 * m_out1[i]) -
                (b2 * m_out2[i]);

            m_in2[i] = m_in1[i];
            m_in1[i] = input[i];

            m_out2[i] = m_out1[i];
            m_out1[i] = m_output[i];
        }

        return m_output;
    }

    public void Reset()
    {
        for (int i = 0; i < m_inputLen; i++)
        {
            m_in1[i] = 0;
            m_in2[i] = 0;
            m_out1[i] = 0;
            m_out2[i] = 0;
            m_output[i] = 0;
        }
    }
}

/// <summary>
/// Butterworth 1st order high pass filter.
/// </summary>
public class ButterHPFilter
{
    private int m_inputLen;
    private float m_resonance;
    private float m_cutoffFreq;
    private float[] m_in1;
    private float[] m_in2;
    private float[] m_out1;
    private float[] m_out2;
    private float[] m_output;

    public float[] Output
    {
        get { return m_output; }
    }


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="resonance">Resonance amount, from sqrt(2) to ~ 0.1</param>
    /// <param name="cutoffFreq">Cutoff frequency (from ~0Hz to SampleRate / 2 - though  many synths seem to filter only up to SampleRate/4)</param>
    public ButterHPFilter(int inputLen, float resonance, float cutoffFreq)
    {
        Debug.Assert(inputLen != 0 || inputLen > 0);

        m_inputLen = inputLen;
        m_resonance = resonance;
        m_cutoffFreq = cutoffFreq;

        m_in1 = new float[inputLen];
        m_in2 = new float[inputLen];
        m_out1 = new float[inputLen];
        m_out2 = new float[inputLen];
        m_output = new float[inputLen];

        for (int i = 0; i < inputLen; i++)
        {
            m_in1[i] = 0;
            m_in2[i] = 0;
            m_out1[i] = 0;
            m_out2[i] = 0;
            m_output[i] = 0;
        }
    }

    public void Filter(float[] input, float sampleRate)
    {
        Debug.Assert(input.Length == m_inputLen);

        float c = Mathf.Tan(Mathf.PI * m_cutoffFreq / sampleRate);

        float a1 = 1.0f / (1.0f + m_resonance * c + c * c);
        float a2 = -2.0f * a1;
        float a3 = a1;

        float b1 = 2.0f * (c * c - 1.0f) * a1;
        float b2 = (1.0f - m_resonance * c + c * c) * a1;

        for (int i = 0; i < m_inputLen; i++)
        {
            m_output[i] = (a1 * input[i]) + (a2 * m_in1[i]) + (a3 * m_in2[i]) - (b1 * m_out1[i]) - (b2 * m_out2[i]);

            m_in2[i] = m_in1[i];
            m_in1[i] = input[i];

            m_out2[i] = m_out1[i];
            m_out1[i] = m_output[i];
        }
    }

    public void Reset()
    {
        for (int i = 0; i < m_inputLen; i++)
        {
            m_in1[i] = 0;
            m_in2[i] = 0;
            m_out1[i] = 0;
            m_out2[i] = 0;
            m_output[i] = 0;
        }
    }
}