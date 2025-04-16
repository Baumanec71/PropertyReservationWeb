import { useEffect, useState } from "react";
import { getAdvertisements } from "../../services/advertisements/getAdvertisements";
import { BiLogoBaidu, BiLoaderCircle} from "react-icons/bi";
import AdvertisementCard from "./AdvertisementCard";
import { useParams } from "react-router-dom";
import { HiSearchCircle } from "react-icons/hi";
import {
  Box,
  Grid,
  Text,
  HStack,
  VStack,
  Input,
  Button,
  Select,
  Checkbox,
  CheckboxGroup,
  Stack,
  Collapsible,
  RatingGroup,
  GridItem,
  createListCollection,
  Wrap,
  WrapItem,
} from "@chakra-ui/react";
import {
  PaginationItems,
  PaginationNextTrigger,
  PaginationPrevTrigger,
  PaginationRoot,
} from "@/components/ui/pagination";
import {
  SelectContent,
  SelectItem,
  SelectLabel,
  SelectRoot,
  SelectTrigger,
  SelectValueText,
} from "@/components/ui/select";
import { useNavigate } from "react-router-dom";

export default function GetAdvertisements() {
  const [advertisements, setAdvertisements] = useState([]);
  const navigate = useNavigate();
  const [page, setPage] = useState(1);

  const [totalPages, setTotalPages] = useState(1);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [objectTypeLocal, setObjectTypeLocal] = useState("");
  const [renderKey, setRenderKey] = useState(0);
  const { page: routePage } = useParams();  // получаем номер страницы из URL

  const [filterModel, setFilterModel] = useState({
    selectedObjectType: null,
    selectedAddress: null,
    selectedMinRentalPrice: null,
    selectedMaxRentalPrice: null,
    selectedMinFixedPrepaymentAmount: null,
    selectedMaxFixedPrepaymentAmount: null,
    selectedNumberOfRooms: null,
    selectedNumberOfBeds: null,
    selectedNumberOfBathrooms: null,
    selectedMinRating: null,
    createAdvertisementAmenities: [],
    types: [],
  });

  const handleCheckboxChange = (amenity, checked) => {
    console.log("Изменение чекбокса:", amenity, checked);
    setFilterModel((prev) => ({
      ...prev,
      createAdvertisementAmenities: prev.createAdvertisementAmenities.map((a) =>
        a.amenity === amenity.amenity
          ? {
              ...a,
              isActive: checked,
              value: checked && ![2, 3].includes(a.amenity) ? 1 : a.value,
            }
          : a
      ),
    }));
  };

  const handleObjectTypeChange = (newVal) => {
    setObjectTypeLocal(newVal.value);
    setFilterModel((prev) => ({
      ...prev,
      selectedObjectType: newVal.value === "" ? null : Number(newVal.value),
    }));
  };

  const objectTypeCollection = createListCollection({
    items: filterModel.types.length
      ? filterModel.types.map((option) => ({
          label: option.displayName,
          value: String(option.value),
        }))
      : [
          { label: "Квартира", value: "0" },
          { label: "Дом", value: "1" },
        ],
  });

  const fetchData = async (filters) => {
    try {
      const advertisementsData = await getAdvertisements(page, filters);
      if (advertisementsData?.viewModels) {
        setAdvertisements(advertisementsData.viewModels);
        setTotalPages(advertisementsData.totalPages || 1);
        setFilterModel((prev) => ({
          ...prev,
          types: advertisementsData.filterModel?.types || prev.types,
          createAdvertisementAmenities:
            filters && filters.createAdvertisementAmenities
              ? filters.createAdvertisementAmenities
              : advertisementsData.filterModel?.createAdvertisementAmenities ||
                prev.createAdvertisementAmenities,
        }));
      } else {
        setAdvertisements([]);
        setTotalPages(1);
      }
    } catch (error) {
      console.error("Ошибка при получении данных объявлений:", error);
      setAdvertisements([]);
      setTotalPages(1);
    }
  };

  useEffect(() => {
    fetchData();
  }, [page]);

  useEffect(() => {
    // Если номер страницы в URL существует, используем его
    setPage(Number(routePage) || 1);
  }, [routePage]);

  // Применить фильтры
  const handleApplyFilters = () => {
    setPage(1);
    fetchData(filterModel);
  };

  // Сброс фильтров
  const handleResetFilters = () => {
    const newFilterModel = {
      selectedObjectType: null,
      selectedAddress: null,
      selectedMinRentalPrice: null,
      selectedMaxRentalPrice: null,
      selectedMinFixedPrepaymentAmount: null,
      selectedMaxFixedPrepaymentAmount: null,
      selectedNumberOfRooms: null,
      selectedNumberOfBeds: null,
      selectedNumberOfBathrooms: null,
      selectedMinRating: null,
      createAdvertisementAmenities: filterModel.createAdvertisementAmenities.map((a) => ({
        ...a,
        isActive: false,
      })),
      types: [],
    };
    // Устанавливаем сброшенный фильтр
    setFilterModel(newFilterModel);

    // Инкрементируем renderKey, чтобы CheckboxGroup пересоздался
    setRenderKey((prev) => prev + 1);

    setObjectTypeLocal("");
    setPage(1);
    fetchData(newFilterModel);
  };

  const handleFilterChange = (e) => {
    const { name, value, type } = e.target;
    setFilterModel((prev) => ({
      ...prev,
      [name]: value === "" ? null : type === "number" ? Number(value) : value,
    }));
  };

  const handlePageChange = (newPage) => {
    setPage(newPage);
    navigate(`/Advertisements/${newPage}`);  // переход по маршруту с номером страницы
  };

   // appearance="dark" colorPalette="red" forcedTheme="dark"  bg = "red.500"
  return (
    <Box w="100%" p={4} maxW="1200px" color="black" mx="auto" textAlign="center">
      {/* Верхняя панель поиска */}
      <HStack w="100%" spacing={2} mb={4} justify="center" colorPalette="black">
        <Input
          name="selectedAddress"
          placeholder="Поиск по адресу"
          value={filterModel.selectedAddress ?? ""}
          onChange={handleFilterChange}
          className="w-3/5 p-2 border border-gray-300 rounded-lg shadow-sm"
        />
        <Button
          onClick={() => setIsFilterOpen(!isFilterOpen)}
          px={6}
          py={3}
          rounded="lg"
          bg="#111111"
          //bg="#FFEB3B"
          color="white"
          fontWeight="semibold"
         
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
        >
          <HiSearchCircle />
        </Button>
      </HStack>

      {/* Фильтр */}
      <Collapsible.Root open={isFilterOpen}>
  <Collapsible.Trigger />
  <Collapsible.Content  p={5} px={2} bg="gray.50" borderColor="#FFEB3B"rounded="lg" boxShadow="md" >
    <VStack >
      <HStack w="100%" spacing={4} >
        <SelectRoot
          collection={objectTypeCollection}
          value={Array.isArray(objectTypeLocal) ? objectTypeLocal : [objectTypeLocal]} 
          onValueChange={handleObjectTypeChange}
          size="sm"
          width="full"
        >
          <SelectLabel fontWeight="semibold" color="gray.700"></SelectLabel>
          <SelectTrigger borderColor="gray.300" _hover={{ borderColor: "#FFEB3B" }}>
            <SelectValueText placeholder="Выберите тип объекта" />
          </SelectTrigger>
          <SelectContent>
            {objectTypeCollection.items.map((item) => (
              <SelectItem key={item.value} item={item}>
                {item.label}
              </SelectItem>
            ))}
          </SelectContent>
        </SelectRoot>

        <RatingGroup.Root
          name="selectedMinRating"
          placeholder="Минимальный рейтинг объявления"
          count={5}
          size="md"
          colorPalette = "yellow"
          value={filterModel.selectedMinRating ?? 0}
          onValueChange={(newValue) => {
            const rating = newValue?.value ?? 0;
            setFilterModel((prev) => ({ ...prev, selectedMinRating: rating }));
          }}
        >
          <RatingGroup.HiddenInput />
          <RatingGroup.Control />
        </RatingGroup.Root>
      </HStack>

      <HStack w="100%" spacing={4}>
        <Input
          name="selectedMinRentalPrice"
          type="number"
          placeholder="Цена от"
          value={filterModel.selectedMinRentalPrice ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "#FFEB3B" }}
        />
        <Input
          name="selectedMaxRentalPrice"
          type="number"
          placeholder="Цена до"
          value={filterModel.selectedMaxRentalPrice ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "#FFEB3B" }}
        />
      </HStack>

      <HStack w="100%" spacing={4}>
        <Input
          name="selectedNumberOfRooms"
          type="number"
          placeholder="Количество комнат"
          value={filterModel.selectedNumberOfRooms ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "#FFEB3B" }}
        />
        <Input
          name="selectedNumberOfBeds"
          type="number"
          placeholder="Число спальных мест"
          value={filterModel.selectedNumberOfBeds ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "#FFEB3B" }}
        />
      </HStack>

      <HStack w="100%" spacing={4}>
        <Input
          name="selectedNumberOfBathrooms"
          type="number"
          placeholder="Число ванных комнат"
          value={filterModel.selectedNumberOfBathrooms ?? ""}
          onChange={handleFilterChange}
          borderColor="gray.300"
          _focus={{ borderColor: "#FFEB3B" }}
        />
      </HStack>
      <CheckboxGroup name="Удобства" key={renderKey}>
  <VStack align="start" w="100%" spacing={4}>
    <Text fontSize="lg" fontWeight="semibold" color="gray.700">
      Удобства
    </Text>

    <Grid
      templateColumns={{
        base: "repeat(1, 1fr)",
        sm: "repeat(2, 1fr)",
        md: "repeat(3, 1fr)",
      }}
      gap={4}
      rowGap={5}
      w="100%"
      bg="white"
      p={4}
      rounded="xl"
      boxShadow="sm"
      border="1px solid"
      borderColor="gray.200"
    >
      {(filterModel.createAdvertisementAmenities || []).map((amenity) => (
        <GridItem key={amenity.amenity}>
          <Checkbox.Root
            value={amenity.amenity}
            checked={amenity.isActive || false}
            onCheckedChange={(e) =>
              handleCheckboxChange(amenity, !!e.checked)
            }
          >
            <Checkbox.HiddenInput />
            <HStack spacing={3}>
              <Checkbox.Control
                color="black"
                borderColor="gray.300"
                _checked={{ bg: "#FFEB3B", borderColor: "black" }}
                _hover={{ borderColor: "#FFEB3B" }}
                boxSize={5}
              />
              <Checkbox.Label fontSize="sm" color="gray.700">
                {amenity.amenityDisplay}
              </Checkbox.Label>
            </HStack>
          </Checkbox.Root>
        </GridItem>
      ))}
    </Grid>
  </VStack>
</CheckboxGroup>
     

      <HStack w="100%" justifyContent="space-between" spacing={4}>
        <Button
          onClick={handleApplyFilters}
          color="white"
          px={6}
          py={3}
          rounded="lg"
          size="lg"
          mt={4}
          bg="#111111"
          //bg="#FFEB3B"
          fontWeight="semibold"
         
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
        >
          Поиск
        </Button>
        <Button
          onClick={handleResetFilters}
          bg="gray.400"
          color="white"
          px={6}
          py={3}
          rounded="lg"
          _hover={{ bg: "gray.500" }}
        >
          Сбросить
        </Button>
      </HStack>
    </VStack>
  </Collapsible.Content>
</Collapsible.Root>

      {/* Список объявлений */}
      {advertisements.length > 0 ? (
        <>
<Grid
            templateColumns={{
              base: "repeat(auto-fit, minmax(280px, 1fr))",  
              sm: "repeat(auto-fit, minmax(300px, 1fr))",    
              md: "repeat(auto-fit, minmax(350px, 1fr))",    
              lg: "repeat(auto-fit, minmax(400px, 1fr))",    
              xl: "repeat(auto-fit, minmax(450px, 1fr))",    
              "2xl": "repeat(auto-fit, minmax(500px, 1fr))",        
            }} 
            gridAutoRows="1fr"  
            gap={6}      
            rowGap={8}  
            alignItems="stretch"
            w="100%"
>
            {advertisements.map((advertisement) => (
              <AdvertisementCard key={advertisement.id} ad={advertisement} />
            ))}
          </Grid>
          <PaginationRoot
            count={totalPages}
            value={page}
            pageSize={1}
            onPageChange={(e) => handlePageChange(e.page)}
          >
            <HStack justifyContent="center" color = "black" mt={4}>
              <PaginationPrevTrigger color = "black"/>
              <PaginationItems color = "black"/>
              <PaginationNextTrigger color = "black"/>
            </HStack>
          </PaginationRoot>
        </>
      ) : (
        <Text textAlign="center" mt={4}>
          Объявлений пока нет
        </Text>
      )}
    </Box>
  );
}