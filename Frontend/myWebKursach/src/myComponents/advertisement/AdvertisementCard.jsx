import { Card, Stack, Text, Button, Image, IconButton } from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";
import { BiLeftArrowAlt, BiRightArrowAlt } from 'react-icons/bi'
import { useState } from "react";

export default function AdvertisementCard({ ad }) {
    const navigate = useNavigate();
    const [currentPhotoIndex, setCurrentPhotoIndex] = useState(0);

    const nextPhoto = () => {
        setCurrentPhotoIndex((prevIndex) => (prevIndex + 1) % ad.photos.length);
    };

    const prevPhoto = () => {
        setCurrentPhotoIndex((prevIndex) => (prevIndex - 1 + ad.photos.length) % ad.photos.length);
    };

    return (
        <Card.Root className="w-full md:w-80 shadow-lg hover:shadow-xl transition">
 <Card.Header>
                {ad.photos.length > 0 && (
                    <div style={{ position: "relative", width: "100%", height: "192px" }}>
                        <Image
                            src={ad.photos[currentPhotoIndex].valuePhoto}
                            alt="Фото"
                            w="100%"
                            h="192px"
                            objectFit="cover"
                            borderRadius="md"
                        />
                        {ad.photos.length > 1 && (
                            <>
                                <IconButton
                                    icon={<BiLeftArrowAlt />}
                                    position="absolute"
                                    top="50%"
                                    left="2"
                                    transform="translateY(-50%)"
                                    onClick={prevPhoto}
                                    size="sm"
                                    variant="ghost"
                                    aria-label="Предыдущее фото"
                                />
                                <IconButton
                                    icon={<BiRightArrowAlt />}
                                    position="absolute"
                                    top="50%"
                                    right="2"
                                    transform="translateY(-50%)"
                                    onClick={nextPhoto}
                                    size="sm"
                                    variant="ghost"
                                    aria-label="Следующее фото"
                                />
                            </>
                        )}
                    </div>
                )}
                <Card.Title>{ad.objectType}</Card.Title>
            </Card.Header>
            <Card.Body>
            <Text><b>Адрес:</b> {ad.adressName}</Text>
            <Text><b>Стоимость:</b> {ad.rentalPrice} ₽</Text>
            <Text><b>Рейтинг:</b> {ad.rating.toFixed(1)}</Text>
            </Card.Body>
            <Card.Footer>
                <Button onClick={() => navigate(`/advertisements/${ad.id}`)}>Подробнее</Button>
            </Card.Footer>
        </Card.Root>
    );
};