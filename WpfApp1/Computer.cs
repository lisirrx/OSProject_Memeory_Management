using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace WpfApp1 {
    class Computer : INotifyPropertyChanged {
        Memory memory;
        PageTable pageTable;
        Disk disk;
        Instruction current;
        Instruction PC;

        int CurrentInstruction;
        int NextInstruction;
        int accessCount;
        int pageFaultCount;
        double rate;

        int Time = 0;
        int[] blocks = new int[4];

        public event PropertyChangedEventHandler PropertyChanged;

        public Computer(){
            pageTable = new PageTable();
            memory = new Memory();
            disk = new Disk();
        }






        public void start() {
            Random ran = new Random();
            int n = ran.Next(0, 320);
            current = accessMemory(n);

            for(int i = 0; i < 320; i++) {
                Time++;
                switch(i % 4) {
                    case 0:
                    case 3:
                        n = current.Number + 1;
                        if(n < 320) {
                            PC = accessMemory(n);
                        }
                        break;
                    case 1:
                        n = ran.Next(0, current.Number);
                        PC = accessMemory(n);
                        break;
                    case 2:
                        n = ran.Next(current.Number, 319);
                        PC = accessMemory(n);
                        break;
                }

                Rate = memory.Rate;
                Current = current.Number;
                Next = PC.Number;
                Frame0 = memory.getFrames()[0] == null ? -1 : memory.getFrames()[0].PageNumber;
                Frame1 = memory.getFrames()[1] == null ? -1 : memory.getFrames()[1].PageNumber;
                Frame2 = memory.getFrames()[2] == null ? -1 : memory.getFrames()[2].PageNumber;
                Frame3 = memory.getFrames()[3] == null ? -1 : memory.getFrames()[3].PageNumber;
                AccessCount = memory.AccessCount;
                PageFaultCount = memory.PageFault;

                Thread.Sleep(100);
                current = PC;
            }
        }

        public Instruction accessMemory(int address)  {
            int pageNumber = address / 10;
            int offset = address % 10;
            int frameNumber = pageTable.getPage(pageNumber);
            if(frameNumber == -1) {
                memory.PageFault++;

                Block[] frames = memory.getFrames();
                Block LNU = frames[0];
                int replacedIndedx = 0;

                for(int i = 0; i < 4; ++i) {
                    if(frames[i] == null || LNU == null) {
                        replacedIndedx = i;
                        break;
                    } else if(frames[i].TimeCount < LNU.TimeCount) {
                        replacedIndedx = i;
                    }
                }
                if(frames[replacedIndedx] != null) {
                    pageTable.setMapping(frames[replacedIndedx].PageNumber, -1);
                }
                frames[replacedIndedx] = disk.getBlock(pageNumber);
                pageTable.setMapping(pageNumber, replacedIndedx);
                memory.setRate();
                return memory.access(replacedIndedx, offset, Time); ;

            } else {
                return memory.access(frameNumber, offset, Time);
            }
        }

        public int AccessCount {
            get { return accessCount; }
            set { accessCount = value; PropertyChanged(this, new PropertyChangedEventArgs("AccessCount")); }
        }


        public int PageFaultCount {
            get { return pageFaultCount; }
            set { pageFaultCount = value; PropertyChanged(this, new PropertyChangedEventArgs("PageFaultCount")); }
        }



        public int Frame0 {
            get { return blocks[0]; }
            set { blocks[0] = value; PropertyChanged(this, new PropertyChangedEventArgs("Frame0")); }
        }

        public int Frame1 {
            get { return blocks[1]; }
            set { blocks[1] = value; PropertyChanged(this, new PropertyChangedEventArgs("Frame1")); }
        }

        public int Frame2 {
            get { return blocks[2]; }
            set { blocks[2] = value; PropertyChanged(this, new PropertyChangedEventArgs("Frame2")); }
        }

        public int Frame3 {
            get { return blocks[3]; }
            set { blocks[3] = value; PropertyChanged(this, new PropertyChangedEventArgs("Frame3")); }
        }

        public double Rate {
            get { return rate; }
            set { rate = Math.Round(value, 2); PropertyChanged(this, new PropertyChangedEventArgs("Rate")); }
        }

        public int Current {
            get { return CurrentInstruction; }
            set { CurrentInstruction = value; PropertyChanged(this, new PropertyChangedEventArgs("Current")); }
        }

        public int Next {
            get { return NextInstruction; }
            set { NextInstruction = value; PropertyChanged(this, new PropertyChangedEventArgs("Next")); }
        }

    }


    class Memory  {
        Block[] frames = new Block[4];
        int accessCount = 0;
        int pageFault = 0;
        double pageFaultRate;


        public double Rate{ get { return pageFaultRate; } }

        public Block[] getFrames(){ return frames; }

        public int AccessCount { get { return accessCount;}set { accessCount = value; }  }

        public int PageFault {  get { return pageFault;  }  set {      pageFault = value;  }  }

        public Memory() { }

        public void setBlock(int frameNumber, Block block){
            frames[frameNumber] = block;
        }


        public Instruction access(int frameNumber, int offset, int time) {
            frames[frameNumber].TimeCount = time;
            Instruction t = frames[frameNumber].getInstruction(offset);
            return t;
        }

        public void setRate() {
            pageFaultRate = (double)pageFault / (double)accessCount;
        }

    }


    class PageTable{
        int[] pageMapping = Enumerable.Repeat(-1, 32).ToArray();

        public PageTable() { }

        public void setMapping(int[] pageNumbers, int[] frameNumbers)  {
            for(int i = 0; i < pageNumbers.Length; i++) {
                pageMapping[i] = frameNumbers[i];
            }
        }

        public void setMapping(int pageNumber, int frameNumber)  {
            pageMapping[pageNumber] = frameNumber;
        }

        public int getPage(int pageNumber) {
            return pageMapping[pageNumber];
        }


    }


    class Disk {
        Block[] swapBlocks = new Block[32];

        public Disk() {

            for(int i = 0; i < 32; i++) {
                swapBlocks[i] = new Block(i);
            }
        }


        public Block getBlock(int pageNumber) {
            return swapBlocks[pageNumber];
        }



    }

    class Block {
        int pageNumber;
        Instruction[] instructions = new Instruction[10];
        int timeCount = 0;

        public int PageNumber { get {  return pageNumber; } set { pageNumber = value;} }


        public int TimeCount{  get { return timeCount;  } set { timeCount = value; }}


        public Block(int pageCount)  {
            pageNumber = pageCount;
            for(int i = 0; i < 10; i++) {
                instructions[i] = new Instruction(10 * pageCount + i);
            }
        }


        public Instruction getInstruction(int offset) {
            return instructions[offset];
        }

    }

    class Instruction  {
        int number;

        public Instruction(int n)  {
            number = n;
        }

        public int Number  {
            get { return number; }
            private set { }
        }

    }


}
