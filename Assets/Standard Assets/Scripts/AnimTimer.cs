using System;

public struct AnimTimer
{
	public bool Enabled
	{
		get
		{
			return this.enabled;
		}
	}

	public bool Running
	{
		get
		{
			return this.running;
		}
	}

	public bool Completed
	{
		get
		{
			return !this.running;
		}
	}

	public float Elapsed
	{
		get
		{
			return this.elapsed;
		}
	}

	public float Duration
	{
		get
		{
			return this.duration;
		}
	}

	public float Nt
	{
		get
		{
			return this.nt;
		}
	}

	public float NtPrev
	{
		get
		{
			return this.ntPrev;
		}
	}

	public void Reset(float t = 0f)
	{
		this.enabled = false;
		this.running = false;
		this.elapsed = 0f;
		this.nt = t;
		this.ntPrev = t;
	}

	public void Start(float duration)
	{
		this.enabled = true;
		this.running = true;
		this.nt = 0f;
		this.ntPrev = 0f;
		this.elapsed = 0f;
		this.duration = ((duration <= 0f) ? 0f : duration);
	}

	public void Update(float dt)
	{
		if (!this.enabled)
		{
			return;
		}
		this.ntPrev = this.nt;
		if (this.running)
		{
			this.elapsed += dt;
			if (this.elapsed > this.duration)
			{
				this.nt = 1f;
				this.running = false;
			}
			else if (this.duration > 0.0001f)
			{
				this.nt = this.elapsed / this.duration;
			}
			else
			{
				this.nt = 0f;
			}
		}
	}

	public void Disable()
	{
		this.enabled = false;
		this.running = false;
	}

	private bool enabled;

	private bool running;

	private float elapsed;

	private float duration;

	private float nt;

	private float ntPrev;
}
