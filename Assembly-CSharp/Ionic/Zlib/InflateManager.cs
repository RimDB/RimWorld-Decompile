﻿using System;

namespace Ionic.Zlib
{
	internal sealed class InflateManager
	{
		private const int PRESET_DICT = 32;

		private const int Z_DEFLATED = 8;

		private InflateManager.InflateManagerMode mode;

		internal ZlibCodec _codec;

		internal int method;

		internal uint computedCheck;

		internal uint expectedCheck;

		internal int marker;

		private bool _handleRfc1950HeaderBytes = true;

		internal int wbits;

		internal InflateBlocks blocks;

		private static readonly byte[] mark = new byte[]
		{
			0,
			0,
			byte.MaxValue,
			byte.MaxValue
		};

		public InflateManager()
		{
		}

		public InflateManager(bool expectRfc1950HeaderBytes)
		{
			this._handleRfc1950HeaderBytes = expectRfc1950HeaderBytes;
		}

		internal bool HandleRfc1950HeaderBytes
		{
			get
			{
				return this._handleRfc1950HeaderBytes;
			}
			set
			{
				this._handleRfc1950HeaderBytes = value;
			}
		}

		internal int Reset()
		{
			this._codec.TotalBytesIn = (this._codec.TotalBytesOut = 0L);
			this._codec.Message = null;
			this.mode = ((!this.HandleRfc1950HeaderBytes) ? InflateManager.InflateManagerMode.BLOCKS : InflateManager.InflateManagerMode.METHOD);
			this.blocks.Reset();
			return 0;
		}

		internal int End()
		{
			if (this.blocks != null)
			{
				this.blocks.Free();
			}
			this.blocks = null;
			return 0;
		}

		internal int Initialize(ZlibCodec codec, int w)
		{
			this._codec = codec;
			this._codec.Message = null;
			this.blocks = null;
			if (w < 8 || w > 15)
			{
				this.End();
				throw new ZlibException("Bad window size.");
			}
			this.wbits = w;
			this.blocks = new InflateBlocks(codec, (!this.HandleRfc1950HeaderBytes) ? null : this, 1 << w);
			this.Reset();
			return 0;
		}

		internal int Inflate(FlushType flush)
		{
			if (this._codec.InputBuffer == null)
			{
				throw new ZlibException("InputBuffer is null. ");
			}
			int num = 0;
			int num2 = -5;
			for (;;)
			{
				switch (this.mode)
				{
				case InflateManager.InflateManagerMode.METHOD:
					if (this._codec.AvailableBytesIn == 0)
					{
						goto Block_3;
					}
					num2 = num;
					this._codec.AvailableBytesIn--;
					this._codec.TotalBytesIn += 1L;
					if (((this.method = (int)this._codec.InputBuffer[this._codec.NextIn++]) & 15) != 8)
					{
						this.mode = InflateManager.InflateManagerMode.BAD;
						this._codec.Message = string.Format("unknown compression method (0x{0:X2})", this.method);
						this.marker = 5;
						continue;
					}
					if ((this.method >> 4) + 8 > this.wbits)
					{
						this.mode = InflateManager.InflateManagerMode.BAD;
						this._codec.Message = string.Format("invalid window size ({0})", (this.method >> 4) + 8);
						this.marker = 5;
						continue;
					}
					this.mode = InflateManager.InflateManagerMode.FLAG;
					continue;
				case InflateManager.InflateManagerMode.FLAG:
				{
					if (this._codec.AvailableBytesIn == 0)
					{
						goto Block_6;
					}
					num2 = num;
					this._codec.AvailableBytesIn--;
					this._codec.TotalBytesIn += 1L;
					int num3 = (int)(this._codec.InputBuffer[this._codec.NextIn++] & byte.MaxValue);
					if (((this.method << 8) + num3) % 31 != 0)
					{
						this.mode = InflateManager.InflateManagerMode.BAD;
						this._codec.Message = "incorrect header check";
						this.marker = 5;
						continue;
					}
					this.mode = (((num3 & 32) != 0) ? InflateManager.InflateManagerMode.DICT4 : InflateManager.InflateManagerMode.BLOCKS);
					continue;
				}
				case InflateManager.InflateManagerMode.DICT4:
					if (this._codec.AvailableBytesIn == 0)
					{
						goto Block_9;
					}
					num2 = num;
					this._codec.AvailableBytesIn--;
					this._codec.TotalBytesIn += 1L;
					this.expectedCheck = (uint)((long)((long)this._codec.InputBuffer[this._codec.NextIn++] << 24) & (long)((ulong)-16777216));
					this.mode = InflateManager.InflateManagerMode.DICT3;
					continue;
				case InflateManager.InflateManagerMode.DICT3:
					if (this._codec.AvailableBytesIn == 0)
					{
						goto Block_10;
					}
					num2 = num;
					this._codec.AvailableBytesIn--;
					this._codec.TotalBytesIn += 1L;
					this.expectedCheck += (uint)((int)this._codec.InputBuffer[this._codec.NextIn++] << 16 & 16711680);
					this.mode = InflateManager.InflateManagerMode.DICT2;
					continue;
				case InflateManager.InflateManagerMode.DICT2:
					if (this._codec.AvailableBytesIn == 0)
					{
						goto Block_11;
					}
					num2 = num;
					this._codec.AvailableBytesIn--;
					this._codec.TotalBytesIn += 1L;
					this.expectedCheck += (uint)((int)this._codec.InputBuffer[this._codec.NextIn++] << 8 & 65280);
					this.mode = InflateManager.InflateManagerMode.DICT1;
					continue;
				case InflateManager.InflateManagerMode.DICT1:
					goto IL_3C3;
				case InflateManager.InflateManagerMode.DICT0:
					goto IL_45B;
				case InflateManager.InflateManagerMode.BLOCKS:
					num2 = this.blocks.Process(num2);
					if (num2 == -3)
					{
						this.mode = InflateManager.InflateManagerMode.BAD;
						this.marker = 0;
						continue;
					}
					if (num2 == 0)
					{
						num2 = num;
					}
					if (num2 != 1)
					{
						goto Block_15;
					}
					num2 = num;
					this.computedCheck = this.blocks.Reset();
					if (!this.HandleRfc1950HeaderBytes)
					{
						goto Block_16;
					}
					this.mode = InflateManager.InflateManagerMode.CHECK4;
					continue;
				case InflateManager.InflateManagerMode.CHECK4:
					if (this._codec.AvailableBytesIn == 0)
					{
						goto Block_17;
					}
					num2 = num;
					this._codec.AvailableBytesIn--;
					this._codec.TotalBytesIn += 1L;
					this.expectedCheck = (uint)((long)((long)this._codec.InputBuffer[this._codec.NextIn++] << 24) & (long)((ulong)-16777216));
					this.mode = InflateManager.InflateManagerMode.CHECK3;
					continue;
				case InflateManager.InflateManagerMode.CHECK3:
					if (this._codec.AvailableBytesIn == 0)
					{
						goto Block_18;
					}
					num2 = num;
					this._codec.AvailableBytesIn--;
					this._codec.TotalBytesIn += 1L;
					this.expectedCheck += (uint)((int)this._codec.InputBuffer[this._codec.NextIn++] << 16 & 16711680);
					this.mode = InflateManager.InflateManagerMode.CHECK2;
					continue;
				case InflateManager.InflateManagerMode.CHECK2:
					if (this._codec.AvailableBytesIn == 0)
					{
						goto Block_19;
					}
					num2 = num;
					this._codec.AvailableBytesIn--;
					this._codec.TotalBytesIn += 1L;
					this.expectedCheck += (uint)((int)this._codec.InputBuffer[this._codec.NextIn++] << 8 & 65280);
					this.mode = InflateManager.InflateManagerMode.CHECK1;
					continue;
				case InflateManager.InflateManagerMode.CHECK1:
					if (this._codec.AvailableBytesIn == 0)
					{
						goto Block_20;
					}
					num2 = num;
					this._codec.AvailableBytesIn--;
					this._codec.TotalBytesIn += 1L;
					this.expectedCheck += (uint)(this._codec.InputBuffer[this._codec.NextIn++] & byte.MaxValue);
					if (this.computedCheck != this.expectedCheck)
					{
						this.mode = InflateManager.InflateManagerMode.BAD;
						this._codec.Message = "incorrect data check";
						this.marker = 5;
						continue;
					}
					goto IL_740;
				case InflateManager.InflateManagerMode.DONE:
					goto IL_750;
				case InflateManager.InflateManagerMode.BAD:
					goto IL_758;
				}
				break;
			}
			throw new ZlibException("Stream error.");
			Block_3:
			return num2;
			Block_6:
			return num2;
			Block_9:
			return num2;
			Block_10:
			return num2;
			Block_11:
			return num2;
			IL_3C3:
			if (this._codec.AvailableBytesIn == 0)
			{
				return num2;
			}
			this._codec.AvailableBytesIn--;
			this._codec.TotalBytesIn += 1L;
			this.expectedCheck += (uint)(this._codec.InputBuffer[this._codec.NextIn++] & byte.MaxValue);
			this._codec._Adler32 = this.expectedCheck;
			this.mode = InflateManager.InflateManagerMode.DICT0;
			return 2;
			IL_45B:
			this.mode = InflateManager.InflateManagerMode.BAD;
			this._codec.Message = "need dictionary";
			this.marker = 0;
			return -2;
			Block_15:
			return num2;
			Block_16:
			this.mode = InflateManager.InflateManagerMode.DONE;
			return 1;
			Block_17:
			return num2;
			Block_18:
			return num2;
			Block_19:
			return num2;
			Block_20:
			return num2;
			IL_740:
			this.mode = InflateManager.InflateManagerMode.DONE;
			return 1;
			IL_750:
			return 1;
			IL_758:
			throw new ZlibException(string.Format("Bad state ({0})", this._codec.Message));
		}

		internal int SetDictionary(byte[] dictionary)
		{
			int start = 0;
			int num = dictionary.Length;
			if (this.mode != InflateManager.InflateManagerMode.DICT0)
			{
				throw new ZlibException("Stream error.");
			}
			int result;
			if (Adler.Adler32(1u, dictionary, 0, dictionary.Length) != this._codec._Adler32)
			{
				result = -3;
			}
			else
			{
				this._codec._Adler32 = Adler.Adler32(0u, null, 0, 0);
				if (num >= 1 << this.wbits)
				{
					num = (1 << this.wbits) - 1;
					start = dictionary.Length - num;
				}
				this.blocks.SetDictionary(dictionary, start, num);
				this.mode = InflateManager.InflateManagerMode.BLOCKS;
				result = 0;
			}
			return result;
		}

		internal int Sync()
		{
			if (this.mode != InflateManager.InflateManagerMode.BAD)
			{
				this.mode = InflateManager.InflateManagerMode.BAD;
				this.marker = 0;
			}
			int num;
			int result;
			if ((num = this._codec.AvailableBytesIn) == 0)
			{
				result = -5;
			}
			else
			{
				int num2 = this._codec.NextIn;
				int num3 = this.marker;
				while (num != 0 && num3 < 4)
				{
					if (this._codec.InputBuffer[num2] == InflateManager.mark[num3])
					{
						num3++;
					}
					else if (this._codec.InputBuffer[num2] != 0)
					{
						num3 = 0;
					}
					else
					{
						num3 = 4 - num3;
					}
					num2++;
					num--;
				}
				this._codec.TotalBytesIn += (long)(num2 - this._codec.NextIn);
				this._codec.NextIn = num2;
				this._codec.AvailableBytesIn = num;
				this.marker = num3;
				if (num3 != 4)
				{
					result = -3;
				}
				else
				{
					long totalBytesIn = this._codec.TotalBytesIn;
					long totalBytesOut = this._codec.TotalBytesOut;
					this.Reset();
					this._codec.TotalBytesIn = totalBytesIn;
					this._codec.TotalBytesOut = totalBytesOut;
					this.mode = InflateManager.InflateManagerMode.BLOCKS;
					result = 0;
				}
			}
			return result;
		}

		internal int SyncPoint(ZlibCodec z)
		{
			return this.blocks.SyncPoint();
		}

		// Note: this type is marked as 'beforefieldinit'.
		static InflateManager()
		{
		}

		private enum InflateManagerMode
		{
			METHOD,
			FLAG,
			DICT4,
			DICT3,
			DICT2,
			DICT1,
			DICT0,
			BLOCKS,
			CHECK4,
			CHECK3,
			CHECK2,
			CHECK1,
			DONE,
			BAD
		}
	}
}
