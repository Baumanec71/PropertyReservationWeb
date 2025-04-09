import {
    Box,
    Button,
    Heading,
    Input,
    Text,
    useBreakpointValue,
    Fieldset,
    RatingGroup
} from "@chakra-ui/react";
import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { createReview } from "../../services/reviews/createReview"; // Убедись в правильном пути
import { Field } from "@/components/ui/field";


export default function CreateReview() {
    const scale = useBreakpointValue({
        base: "0em",
        sm: "30em",
        md: "48em",
        lg: "62em",
        xl: "80em",
        "2xl": "96em"
    });

    const [rating, setRating] = useState(0);
    const [comment, setComment] = useState("");
    //const [rentalId, setRentalId] = useState("");
    const [errors, setErrors] = useState([]);
    const [okMessage, setOkMessage] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();
     const { idNeedRentalRequest } = useParams();

    const handleSubmit = async () => {
        if (!idNeedRentalRequest) {
            console.log("ID запроса не найдено");
            setError("ID запроса не найдено.");
            return;
        }
        setErrors([]);
        setOkMessage("");
        setIsLoading(true);

        const reviewData = {
            theQualityOfTheTransaction: Number(rating),
            comment: comment,
            idRental: idNeedRentalRequest
        };
        console.log(reviewData);

        const result = await createReview(reviewData);

        setIsLoading(false);

        if (!result.success) {
            setErrors(result.errors || ["Произошла неизвестная ошибка."]);
        } else {
            setOkMessage("Отзыв успешно отправлен!");
            setRating(0);
            setComment("");
            setRentalId("");
        }
    };
    const color = `${COLOR}`;
    const bg = `${BG}`;
    return (
        <Box
            transform={`scale(${scale})`}
            transition="transform 0.2s ease-in-out"
            position="relative"
            maxW="lg"
            p={8}
            color={color}
            bg = {bg}
            borderWidth={1}
            borderRadius="md"
            mx="auto"
            mt={10}
            boxShadow="lg"
        >
            <Heading as="h2" size="lg" mb={4} textAlign="center">
                Оставить отзыв
            </Heading>

            <RatingGroup.Root
                              name="selectedMinRating"
                              placeholder="Поставьте оценку"
                              count={5}
                              size="md"
                              value={rating ?? 0}
                              colorPalette="yellow"
                              defaultValue={0}
                              onValueChange={(newValue) => {
                                const rating = newValue?.value ?? 0;
                                setRating((rating));
                              }}
                            >
                              <RatingGroup.HiddenInput />
                              <RatingGroup.Control />
                            </RatingGroup.Root>
            <Fieldset.Root size="lg" maxW="md">
                <Fieldset.Content>
                    <Field label="Комментарий">
                        <Input
                            type="text"
                            name="comment"
                            value={comment}
                            onChange={(e) => setComment(e.target.value)}
                        />
                    </Field>
                </Fieldset.Content>

                {/* Успешное сообщение */}
                {okMessage && (
                    <Box mt={4} p={2} bg="green.100" borderRadius="md">
                        <Text color="green.700">{okMessage}</Text>
                    </Box>
                )}

                {/* Ошибки */}
                {errors.length > 0 &&
                    errors.map((err, idx) => (
                        <Text key={idx} color="red.500" fontSize="sm" mt={1}>
                            {err}
                        </Text>
                    ))}

                <Button
                    variant="solid"
                    onClick={handleSubmit}
                    isLoading={isLoading}
                    loadingText="Отправка..."
                    width="full"
                    mt={4}
                    px={6}
                    py={3}
                    rounded="lg"
                    bg="green"
                    color="white"
                >
                    Отправить отзыв
                </Button>

                <Button
                    variant="outline"
                    mt={1}
                    px={6}
                    py={3}
                    rounded="lg"
                    bg="blue"
                    color="white"
                    width="full"
                    onClick={() => navigate("/Advertisements/1")}
                >
                    Назад к объявлениям
                </Button>
            </Fieldset.Root>
        </Box>
    );
}