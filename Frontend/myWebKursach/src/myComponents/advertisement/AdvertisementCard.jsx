import { Card, Stack, Text, Button, Image,HStack, IconButton,Heading,Flex , Grid, GridItem, Tag, Box,  Collapsible,
    RatingGroup  } from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";
import { BiLogoBaidu, BiLoaderCircle} from "react-icons/bi";
import { FaChevronLeft, FaChevronRight } from 'react-icons/fa6'
import { useState, useEffect } from "react";
import { getUserRole } from "../../services/InfoJwt/getUserRole";
import {deleteAdvertisementForAdmin} from "../../services/advertisements/deleteAdvertisementForAdmin";

export default function AdvertisementCard({ ad }) {
  console.log(ad)
    const navigate = useNavigate();
    const [currentPhotoIndex, setCurrentPhotoIndex] = useState(0);
    const [role, setRole] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
    const nextPhoto = () => setCurrentPhotoIndex((prevIndex) => (prevIndex + 1) % ad.photos.length);
    const prevPhoto = () => setCurrentPhotoIndex((prevIndex) => (prevIndex - 1 + ad.photos.length) % ad.photos.length);

   useEffect(() => {
      const checkAuth = () => {
        const token = localStorage.getItem("authToken");
        console.log(token);
        setIsAuthenticated(!!token);
        if (token) {
          setRole(getUserRole());
        } else {
          setRole(null);

        }
      };
  
      checkAuth();
    }, [location]);

    const getRatingColor = (rating) => {
      if (rating >= 4.5) return "green";
      if (rating >= 3.5) return "yellow";
      if (rating >= 2) return "orange";
      if (rating > 0) return "red";
      return "gray";
    };
    const getRatingColor2 = (rating) => {
      if (rating >= 4.5) return "green";
      if (rating >= 3.5) return "yellow.300";
      if (rating >= 2) return "orange";
      if (rating > 0) return "red";
      return "gray";
    };

    return (
<Card.Root bg="white" overflow="hidden" rounded="2xl" shadow="md" borderWidth="1px">
  <Card.Header p={0}>
    <Box position="relative" w="full" h="400px">
      <Image
        src={
          ad.photos.length > 0
            ? ad.photos[currentPhotoIndex].valuePhoto
            : "https://upload.wikimedia.org/wikipedia/ru/b/b1/Виктор_Говорков._Нет%21_%28плакат%29.jpg"
        }
        alt="Фото"
        objectFit="cover"
        w="full"
        h="full"
        transition="all 0.3s ease-in-out"
      />
      {ad.photos.length > 1 && (
        <>
          <Button             
          position="absolute"
            top="50%"
            left="4"
            transform="translateY(-50%)"
            onClick={prevPhoto}
            size="sm"
            bg="gray.800"
            color="white"
            _hover={{ bg: "gray.600" }}
            aria-label="Предыдущее фото">
          <FaChevronLeft />
          </Button>

          <Button             
            position="absolute"
            top="50%"
            right="4"
            transform="translateY(-50%)"
            onClick={nextPhoto}
            size="sm"
            bg="gray.800"
            color="white"
            _hover={{ bg: "gray.600" }}
            aria-label="Следующее фото">
          <FaChevronRight />
          </Button>
        </>
      )}
    </Box>
    <Flex justify="space-between" align="center" p={6} pt={4} wrap="wrap">
    <Heading size="2xl" color="gray.800" maxW="70%">
      {ad.objectType}, {ad.adressName}
    </Heading>

    <Box
      display="flex"
      alignItems="center"
      gap={2}
    >
      <RatingGroup.Root
        name="Рейтинг"
        readOnly
        count={5}
        size="md"
        value={parseFloat(ad.rating.toFixed(1))}
        colorPalette={getRatingColor(ad.rating.toFixed(1))}
      >
        <RatingGroup.HiddenInput />
        <RatingGroup.Control />
      </RatingGroup.Root>
      <Text
        fontSize="lg"
        fontWeight="bold"
        color={getRatingColor2(ad.rating.toFixed(1))}
      >
        {ad.rating.toFixed(1)}
      </Text>
    </Box>
  </Flex>
</Card.Header>

  <Card.Body px={6} py={4}>
    <Stack spacing={3}>
      <Text color="black"><b>Площадь:</b> {ad.totalArea} м²</Text>
      <Text color="black">
        <b>Комнат:</b> {ad.numberOfRooms}, <b>Спальных мест:</b> {ad.numberOfBeds}
      </Text>
      <Text color="black"><b>Санузлы:</b> {ad.numberOfBathrooms}</Text>

      <Text fontSize="lg" fontWeight="bold" color="black">
        Аренда: {ad.rentalPrice} ₽/день
      </Text>

      <Box>
        <Text fontWeight="bold" color="black" mb={1}>Удобства:</Text>
        <Grid templateColumns={{ base: "repeat(2, 1fr)", md: "repeat(3, 1fr)" }} gap={2}>
          {ad.amenityes?.length > 0 ? (
            ad.amenityes.map((a) => (
              <Tag.Root key={a.id} size="lg" bg= "#FDD835" rounded="md">
                <Tag.Label color="black">{a.amenityType}</Tag.Label>
              </Tag.Root>
            ))
          ) : (
            <Text color="gray.500">Не указаны</Text>
          )}
        </Grid>
      </Box>

      <Box>
        <Text color="black" fontWeight="bold" mt={2} mb={1}>Описание:</Text>
        <Text color="gray.700">
          {ad.description.slice(0, 200)}
          {ad.description.length > 200 && '...'}
        </Text>
      </Box>
    </Stack>
  </Card.Body>

  <Card.Footer p={4} flexDirection="column" gap={3}>
    <HStack w="100%" spacing={4}>
    <Button
        w="50%"
        size="lg"
        px={4}
        py={3}
        bg="#111111"
        color="white"
        fontWeight="semibold"
        rounded="lg"
        transition="all 0.3s ease"
        _hover={{
          bg: "#FDD835", // чуть темнее при наведении
          transform: "scale(1.04)",
          color: "black",
          boxShadow: "0 6px 14px rgba(253, 216, 53, 0.35)"
        }}
        _active={{
          transform: "scale(0.98)",
          boxShadow: "0 2px 6px rgba(253, 216, 53, 0.2)"
        }}
        onClick={() => navigate(`/advertisement/${ad.id}`)}
      >
        Подробнее
      </Button>
      <Button
        w="50%"
        px={4}
        py={2}
        size="lg"
        rounded="lg"
        variant="outline"
        borderColor="#E0E0E0"
        color="#111111"
        fontWeight="medium"
        _hover={{
          bg: "#FFF9C4",
          transform: "scale(1.03)"
        }}
        transition="all 0.2s"
        onClick={() => navigate(`/user/${ad.idAuthor}`)}
      >
        Автор
      </Button>
    </HStack>

    {isAuthenticated && role === "Admin" && (
      <Button
      variant="solid"
        w="100%"
        size="lg"
        px={4}
        py={3}
        bg="#111111"
        color="white"
        fontWeight="semibold"
        rounded="lg"
        transition="all 0.3s ease"
        _hover={{
          bg: "red",
          transform: "scale(1.04)",
          color: "black",
        }}
        onClick={async () => {
          const result = await deleteAdvertisementForAdmin(ad.id);
          if (result.success) {
            alert(response.data)
            console.log("Объявление удалено успешно");
          } else {
            console.error("Ошибка при удалении:", result.errors);
          }
        }}
      >
        Удалить объявление
      </Button>
    )}
  </Card.Footer>
</Card.Root>
    );
  }