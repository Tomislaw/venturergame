using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Chunk.ChunkDecorators
{
    internal interface IChunkDecoratorObject
    {
        void SaveChunkDecorator();

        void LoadFromChunkDecorator();
    }
}