using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace temalab.Models
{
    public class EditableTransactionModel : TransactionModel, IEditableObject
    {
        struct TransactionData
        {
            internal string name;
            internal string description;
            internal int? categoryId;
        }

        private bool inEdit = false;
        private TransactionData transactionData = new TransactionData();
        private TransactionData backupData;

        override public string  name
        {   
            get
            {
                return this.transactionData.name;
            }

            set
            {
                this.transactionData.name = value;
            } 
        }
        
        override public string description
        {
            get
            {
                return this.transactionData.description;
            }

            set
            {
                this.transactionData.description = value;
            }
        }

        override public int? categoryId
        {
            get
            {
                return this.transactionData.categoryId;
            }

            set
            {
                this.transactionData.categoryId = value;
            }
        }

        public void BeginEdit()
        {
            if (!inEdit)
            {
                this.backupData = transactionData;
                inEdit = true;
            }
        }

        public void CancelEdit()
        {
            if (inEdit)
            {
                this.transactionData = backupData;
                inEdit = false;
            }
        }

        public void EndEdit()
        {
            if (inEdit)
            {
                backupData = new TransactionData();
                inEdit = false;
            }
        }
    }
}
