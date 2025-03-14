using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace PropertyReservationWeb.Domain.Helpers
{
    public class GeoJsonConverter : JsonConverter<Point>
    {
        public override void WriteJson(JsonWriter writer, Point? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();

                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            writer.WriteValue(value.X);
            writer.WriteValue(value.Y);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override Point? ReadJson(JsonReader reader, Type objectType, Point? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = serializer.Deserialize<dynamic>(reader);

            if (obj == null || obj!.coordinates == null || obj!.coordinates.Count != 2)
            {
                throw new JsonSerializationException("Invalid GeoJSON format.");
            }

            double x = obj!.coordinates[0];
            double y = obj.coordinates[1];

            return new Point(x, y);
        }
    }
}
