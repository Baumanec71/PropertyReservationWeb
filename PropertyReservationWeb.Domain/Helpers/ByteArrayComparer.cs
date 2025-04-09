//namespace PropertyReservationWeb.Domain.Helpers
//{
//    public class ByteArrayComparer : IEqualityComparer<byte[]>
//    {
//        public bool Equals(byte[] x, byte[] y)
//        {
//            if (x == null || y == null) return false;
//            return x.SequenceEqual(y); // Сравнение содержимого массивов
//        }

//        public int GetHashCode(byte[] obj)
//        {
//            if (obj == null) return 0;
//            return obj.Sum(b => b.GetHashCode()); // Упрощенный хэш-код
//        }
//    }
//}
