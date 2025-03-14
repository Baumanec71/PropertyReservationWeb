import { useEffect, useState } from "react";
import { getAdvertisements } from "../../services/advertisements/getAdvertisements";
import AdvertisementCard from "./AdvertisementCard";
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

export default function GetAdvertisements() {
  const [advertisements, setAdvertisements] = useState([]);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [objectTypeLocal, setObjectTypeLocal] = useState("");

  // ВАЖНО: это состояние будем увеличивать при сбросе, чтобы форсировать ререндер
  const [renderKey, setRenderKey] = useState(0);

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
      console.log("Фильтры перед запросом:", filters);
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

  return (
    <Box w="100%" p={4} className="max-w-6xl mx-auto">
      {/* Верхняя панель поиска */}
      <HStack w="100%" spacing={2} mb={4} justify="center">
        <Input
          name="selectedAddress"
          placeholder="Поиск по адресу"
          value={filterModel.selectedAddress ?? ""}
          onChange={handleFilterChange}
          className="w-3/5 p-2 border border-gray-300 rounded-lg shadow-sm"
        />
        <Button
          onClick={() => setIsFilterOpen(!isFilterOpen)}
          className="px-4 py-2 bg-blue-500 text-white rounded-lg shadow-md"
        >
          Фильтр
        </Button>
      </HStack>

      {/* Фильтр */}
      <Collapsible.Root open={isFilterOpen}>
        <Collapsible.Trigger />
        <Collapsible.Content>
          <VStack spacing={3} p={4} className="bg-gray-100 rounded-lg shadow-md">
            <HStack w="100%">
              <SelectRoot
                collection={objectTypeCollection}
                value={objectTypeLocal}
                onValueChange={handleObjectTypeChange}
                size="sm"
                width="320px"
              >
                <SelectLabel>Тип объекта</SelectLabel>
                <SelectTrigger>
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

            <HStack w="100%">
              <Input
                name="selectedMinRentalPrice"
                type="number"
                placeholder="Цена от"
                value={filterModel.selectedMinRentalPrice ?? ""}
                onChange={handleFilterChange}
              />
              <Input
                name="selectedMaxRentalPrice"
                type="number"
                placeholder="Цена до"
                value={filterModel.selectedMaxRentalPrice ?? ""}
                onChange={handleFilterChange}
              />
            </HStack>

            <HStack w="100%">
              <Input
                name="selectedMinFixedPrepaymentAmount"
                type="number"
                placeholder="Предоплата от"
                value={filterModel.selectedMinFixedPrepaymentAmount ?? ""}
                onChange={handleFilterChange}
              />
              <Input
                name="selectedMaxFixedPrepaymentAmount"
                type="number"
                placeholder="Предоплата до"
                value={filterModel.selectedMaxFixedPrepaymentAmount ?? ""}
                onChange={handleFilterChange}
              />
            </HStack>

            <HStack w="100%">
              <Input
                name="selectedNumberOfRooms"
                type="number"
                placeholder="Количество комнат"
                value={filterModel.selectedNumberOfRooms ?? ""}
                onChange={handleFilterChange}
              />
              <Input
                name="selectedNumberOfBeds"
                type="number"
                placeholder="Число спальных мест"
                value={filterModel.selectedNumberOfBeds ?? ""}
                onChange={handleFilterChange}
              />
            </HStack>

            <HStack w="100%">
              <Input
                name="selectedNumberOfBathrooms"
                type="number"
                placeholder="Число ванных комнат"
                value={filterModel.selectedNumberOfBathrooms ?? ""}
                onChange={handleFilterChange}
              />
            </HStack>

            {/* Группа чекбоксов, у которой ключ = renderKey */}
            <HStack w="100%">
              <CheckboxGroup name="Удобства" key={renderKey}>
                <Grid templateColumns="repeat(5, 1fr)" gap={4}>
                  {(filterModel.createAdvertisementAmenities || []).map((amenity) => (
                    <GridItem key={amenity.amenity}>
                      <Checkbox.Root
                        value={amenity.amenity}
                        checked={amenity.isActive || false}
                        onCheckedChange={(e) => handleCheckboxChange(amenity, !!e.checked)}
                      >
                        <Checkbox.HiddenInput />
                        <Checkbox.Control />
                        <Checkbox.Label>{amenity.amenityDisplay}</Checkbox.Label>
                      </Checkbox.Root>
                    </GridItem>
                  ))}
                </Grid>
              </CheckboxGroup>
            </HStack>

            <HStack w="100%" justifyContent="space-between">
              <Button
                onClick={handleApplyFilters}
                className="bg-blue-500 text-white px-4 py-2 rounded-lg shadow-md"
              >
                Применить
              </Button>
              <Button
                onClick={handleResetFilters}
                className="bg-gray-400 text-white px-4 py-2 rounded-lg shadow-md"
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
            templateColumns={{ base: "repeat(auto-fit, minmax(280px, 1fr))" }}
            gap={6}
            mt={4}
          >
            {advertisements.map((advertisement) => (
              <AdvertisementCard key={advertisement.id} ad={advertisement} />
            ))}
          </Grid>
          <PaginationRoot
            count={totalPages}
            value={page}
            pageSize={1}
            onPageChange={(e) => setPage(e.page)}
          >
            <HStack justifyContent="center" mt={4}>
              <PaginationPrevTrigger />
              <PaginationItems />
              <PaginationNextTrigger />
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