import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import axios from "axios";

export default function AdvertisementDetails() {
    const { id } = useParams();
    const [ad, setAd] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchAd = async () => {
            try {
                const token = localStorage.getItem("authToken");
                const response = await axios.get(`https://localhost:7069/api/Advertisement/GetAdvertisement/${id}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                        "Accept": "application/json",
                    },
                });
                setAd(response.data);
            } catch (error) {
                console.error("Ошибка при получении объявления:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchAd();
    }, [id]);

    if (loading) return <p>Загрузка...</p>;
    if (!ad) return <p>Объявление не найдено</p>;

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-2xl font-bold">{ad.objectType}</h1>
            <p><strong>Адрес:</strong> {ad.adressName}, {ad.apartmentNumber}</p>
            <p><strong>Описание:</strong> {ad.description}</p>
            <p><strong>Общая площадь:</strong> {ad.totalArea} м²</p>
            <p><strong>Цена:</strong> {ad.rentalPrice} ₽</p>
            <p><strong>Предоплата:</strong> {ad.fixedPrepaymentAmount} ₽</p>
            <p><strong>Количество комнат:</strong> {ad.numberOfRooms}</p>
            <p><strong>Количество спальных мест:</strong> {ad.numberOfBeds}</p>
            <p><strong>Количество ванных:</strong> {ad.numberOfBathrooms}</p>
            <p><strong>Дата создания:</strong> {ad.dateCreate}</p>

            <h2 className="text-xl font-bold mt-4">Фотографии</h2>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                {ad.photos.map((photo, index) => (
                    <img key={index} src={photo.url} alt={`Фото ${index + 1}`} className="rounded-md w-full h-32 object-cover" />
                ))}
            </div>

            <h2 className="text-xl font-bold mt-4">Удобства</h2>
            <ul>
                {ad.amenityes.map((amenity, index) => (
                    <li key={index}>✅ {amenity.name}</li>
                ))}
            </ul>
        </div>
    );
};
